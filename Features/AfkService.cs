using System;
using System.Collections.Generic;
using System.Threading;
using Exiled.API.Features;
using PlayerRoles;

namespace AfkManager.Features
{
    internal sealed class AfkService
    {
        private readonly Dictionary<int, AfkPlayer> trackedPlayers = new Dictionary<int, AfkPlayer>();
        private readonly object syncRoot = new object();
        private Timer timer;
        private int isChecking;

        private Config Config => Plugin.Instance.Config;

        public void Start()
        {
            Stop();

            int intervalMilliseconds = Math.Max(250, (int)(Config.CheckInterval * 1000f));
            timer = new Timer(_ => QueueCheck(), null, intervalMilliseconds, intervalMilliseconds);
        }

        public void Stop()
        {
            timer?.Dispose();
            timer = null;
            Interlocked.Exchange(ref isChecking, 0);

            lock (syncRoot)
                trackedPlayers.Clear();
        }

        public void Reset(Player player)
        {
            if (player == null)
                return;

            if (ShouldIgnore(player))
            {
                Remove(player);
                return;
            }

            GetSnapshot(player, out float x, out float y, out float z, out float pitch, out float yaw);

            lock (syncRoot)
            {
                if (trackedPlayers.TryGetValue(player.Id, out AfkPlayer state))
                    state.Reset(x, y, z, pitch, yaw);
                else
                    trackedPlayers[player.Id] = new AfkPlayer(x, y, z, pitch, yaw);
            }
        }

        public void Remove(Player player)
        {
            if (player == null)
                return;

            lock (syncRoot)
                trackedPlayers.Remove(player.Id);
        }

        private void QueueCheck()
        {
            if (Interlocked.Exchange(ref isChecking, 1) != 0)
                return;

            CheckAllPlayers();
        }

        private void CheckAllPlayers()
        {
            try
            {
                foreach (Player player in Player.List)
                    CheckPlayer(player);
            }
            catch (Exception exception)
            {
                Log.Error($"AFK check failed: {exception}");
            }
            finally
            {
                Interlocked.Exchange(ref isChecking, 0);
            }
        }

        private void CheckPlayer(Player player)
        {
            if (player == null || !player.IsConnected || ShouldIgnore(player))
            {
                if (player != null)
                    Remove(player);

                return;
            }

            AfkPlayer state;
            lock (syncRoot)
            {
                if (!trackedPlayers.TryGetValue(player.Id, out state))
                {
                    GetSnapshot(player, out float initialX, out float initialY, out float initialZ, out float initialPitch, out float initialYaw);
                    trackedPlayers[player.Id] = new AfkPlayer(initialX, initialY, initialZ, initialPitch, initialYaw);
                    return;
                }
            }

            GetSnapshot(player, out float x, out float y, out float z, out float pitch, out float yaw);

            float deltaX = x - state.LastX;
            float deltaY = y - state.LastY;
            float deltaZ = z - state.LastZ;
            float movementThreshold = Math.Max(0f, Config.MovementThreshold);
            bool moved = (deltaX * deltaX) + (deltaY * deltaY) + (deltaZ * deltaZ) >= movementThreshold * movementThreshold;

            float pitchDelta = SmallestAngleDifference(pitch, state.LastPitch);
            float yawDelta = SmallestAngleDifference(yaw, state.LastYaw);
            float rotationThreshold = Math.Max(0f, Config.RotationThreshold);
            bool lookedAround = Math.Max(pitchDelta, yawDelta) >= rotationThreshold;

            lock (syncRoot)
                state.UpdateSnapshot(x, y, z, pitch, yaw);

            if (moved || lookedAround)
            {
                lock (syncRoot)
                {
                    state.LastActivity = DateTime.UtcNow;
                    state.WarningSent = false;
                }

                return;
            }

            bool usesScpTimers = player.IsScp && player.Role.Type != RoleTypeId.Scp0492;
            float warningAfter = usesScpTimers ? Config.ScpWarningAfter : Config.WarningAfter;
            float moveAfter = usesScpTimers ? Config.ScpMoveAfter : Config.MoveAfter;
            double inactiveSeconds = (DateTime.UtcNow - state.LastActivity).TotalSeconds;

            if (inactiveSeconds >= warningAfter && inactiveSeconds < moveAfter)
            {
                int remainingSeconds = Math.Max(0, (int)Math.Ceiling(moveAfter - inactiveSeconds));
                string warning = BuildWarningMessage(remainingSeconds);

                player.Broadcast(
                    Config.WarningDuration,
                    warning,
                    Broadcast.BroadcastFlags.Normal,
                    true);

                lock (syncRoot)
                    state.WarningSent = true;
            }

            if (inactiveSeconds < moveAfter)
                return;

            string playerName = player.Nickname;
            string userId = player.UserId;
            string roleName = player.Role.Type.ToString();

            Remove(player);
            player.Role.Set(RoleTypeId.Spectator);

            if (usesScpTimers && Config.NotifyAdminsWhenScpMoved)
                NotifyAdmins(playerName, userId, roleName);
        }

        private string BuildWarningMessage(int remainingSeconds)
        {
            int minutes = remainingSeconds / 60;
            int seconds = remainingSeconds % 60;
            string time = $"{minutes:00}:{seconds:00}";
            string configuredMessage = Config.WarningMessage ?? string.Empty;

            // Existing server configs may still contain placeholders from the old progress-bar design.
            // Fall back to the clean layout so those placeholders can never appear in-game.
            if (configuredMessage.Contains("{bar}") || configuredMessage.Contains("{color}"))
            {
                configuredMessage = "<color=#5B8CFF><b>AFK-Erkennung</b></color>\n" +
                                    "<size=26><color=#5B8CFF><b>{time}</b></color></size>\n" +
                                    "<color=#FFFFFF>Bewege dich, um den Timer zurückzusetzen.</color>";
            }

            return configuredMessage.Replace("{time}", time);
        }

        private void NotifyAdmins(string playerName, string userId, string roleName)
        {
            string message = Config.AdminScpMovedMessage
                .Replace("{player}", playerName ?? "Unknown")
                .Replace("{userid}", userId ?? "Unknown")
                .Replace("{role}", roleName ?? "Unknown");

            foreach (Player staff in Player.List)
            {
                if (staff == null || !staff.IsConnected || !staff.RemoteAdminAccess)
                    continue;

                staff.Broadcast(
                    Config.AdminNotificationDuration,
                    message,
                    Broadcast.BroadcastFlags.AdminChat,
                    false);
            }
        }

        private static bool ShouldIgnore(Player player)
        {
            return player.Role.Type == RoleTypeId.Spectator;
        }

        private static void GetSnapshot(Player player, out float x, out float y, out float z, out float pitch, out float yaw)
        {
            var position = player.Position;
            x = position.x;
            y = position.y;
            z = position.z;

            pitch = 0f;
            yaw = 0f;

            try
            {
                var camera = player.CameraTransform;
                if (camera == null)
                    return;

                var angles = camera.eulerAngles;
                pitch = angles.x;
                yaw = angles.y;
            }
            catch
            {
                // Position movement still counts even when camera rotation is unavailable.
            }
        }

        private static float SmallestAngleDifference(float first, float second)
        {
            float difference = Math.Abs(first - second) % 360f;
            return difference > 180f ? 360f - difference : difference;
        }
    }
}