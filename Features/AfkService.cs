using System;
using System.Collections.Generic;
using Exiled.API.Features;
using MEC;
using PlayerRoles;
using UnityEngine;

namespace AfkManager.Features
{
    internal sealed class AfkService
    {
        private readonly Dictionary<int, AfkPlayer> trackedPlayers = new Dictionary<int, AfkPlayer>();
        private CoroutineHandle coroutine;

        private Config Config => Plugin.Instance.Config;

        public void Start()
        {
            Stop();
            coroutine = Timing.RunCoroutine(CheckLoop());
        }

        public void Stop()
        {
            if (coroutine.IsRunning)
                Timing.KillCoroutines(coroutine);

            trackedPlayers.Clear();
        }

        public void Reset(Player player)
        {
            if (player == null)
                return;

            if (ShouldIgnore(player))
            {
                trackedPlayers.Remove(player.Id);
                return;
            }

            Quaternion rotation = GetRotation(player);

            if (trackedPlayers.TryGetValue(player.Id, out AfkPlayer state))
                state.Reset(player.Position, rotation);
            else
                trackedPlayers[player.Id] = new AfkPlayer(player.Position, rotation);
        }

        public void Remove(Player player)
        {
            if (player != null)
                trackedPlayers.Remove(player.Id);
        }

        private IEnumerator<float> CheckLoop()
        {
            while (true)
            {
                yield return Timing.WaitForSeconds(Mathf.Max(0.25f, Config.CheckInterval));

                foreach (Player player in Player.List)
                    CheckPlayer(player);
            }
        }

        private void CheckPlayer(Player player)
        {
            if (player == null || !player.IsConnected || ShouldIgnore(player))
            {
                if (player != null)
                    trackedPlayers.Remove(player.Id);

                return;
            }

            if (!trackedPlayers.TryGetValue(player.Id, out AfkPlayer state))
            {
                Reset(player);
                return;
            }

            Vector3 position = player.Position;
            Quaternion rotation = GetRotation(player);

            bool moved = (position - state.LastPosition).sqrMagnitude >= Config.MovementThreshold * Config.MovementThreshold;
            bool lookedAround = Quaternion.Angle(rotation, state.LastRotation) >= Config.RotationThreshold;

            state.LastPosition = position;
            state.LastRotation = rotation;

            if (moved || lookedAround)
            {
                state.LastActivity = DateTime.UtcNow;
                state.WarningSent = false;
                return;
            }

            bool isScp = player.IsScp;
            float warningAfter = isScp ? Config.ScpWarningAfter : Config.WarningAfter;
            float moveAfter = isScp ? Config.ScpMoveAfter : Config.MoveAfter;
            double inactiveSeconds = (DateTime.UtcNow - state.LastActivity).TotalSeconds;

            if (!state.WarningSent && inactiveSeconds >= warningAfter)
            {
                player.Broadcast(Config.WarningDuration, Config.WarningMessage, Broadcast.BroadcastFlags.Normal, true);
                state.WarningSent = true;
            }

            if (inactiveSeconds < moveAfter)
                return;

            string playerName = player.Nickname;
            string userId = player.UserId;
            string roleName = player.Role.Type.ToString();

            player.Broadcast(5, Config.MovedMessage, Broadcast.BroadcastFlags.Normal, true);
            trackedPlayers.Remove(player.Id);
            player.Role.Set(RoleTypeId.Spectator);

            if (isScp && Config.NotifyAdminsWhenScpMoved)
                NotifyAdmins(playerName, userId, roleName);
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
            return player.IsOverwatch || player.Role.Type == RoleTypeId.Spectator;
        }

        private static Quaternion GetRotation(Player player)
        {
            return player.CameraTransform != null ? player.CameraTransform.rotation : Quaternion.identity;
        }
    }
}