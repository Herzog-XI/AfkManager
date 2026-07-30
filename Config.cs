using System.ComponentModel;
using Exiled.API.Interfaces;

namespace AfkManager
{
    public sealed class Config : IConfig
    {
        [Description("Whether the plugin is enabled.")]
        public bool IsEnabled { get; set; } = true;

        [Description("Whether debug logging is enabled.")]
        public bool Debug { get; set; } = false;

        [Description("Seconds of inactivity before the warning is shown to non-SCP players.")]
        public float WarningAfter { get; set; } = 60f;

        [Description("Seconds of inactivity before a non-SCP player is moved to Spectator.")]
        public float MoveAfter { get; set; } = 120f;

        [Description("Seconds of inactivity before the warning is shown to SCP players.")]
        public float ScpWarningAfter { get; set; } = 30f;

        [Description("Seconds of inactivity before an SCP player is moved to Spectator.")]
        public float ScpMoveAfter { get; set; } = 60f;

        [Description("How often player activity is checked, in seconds.")]
        public float CheckInterval { get; set; } = 1f;

        [Description("Minimum movement distance that counts as activity.")]
        public float MovementThreshold { get; set; } = 0.05f;

        [Description("Minimum camera rotation in degrees that counts as activity.")]
        public float RotationThreshold { get; set; } = 1f;

        [Description("Duration of each refreshed AFK warning broadcast. Keep this slightly above CheckInterval.")]
        public ushort WarningDuration { get; set; } = 2;

        [Description("Persistent blue-and-white AFK warning. Available placeholder: {time}.")]
        public string WarningMessage { get; set; } = "<color=#5B8CFF><b>AFK-Erkennung</b></color>\n<size=26><color=#5B8CFF><b>{time}</b></color></size>\n<color=#FFFFFF>Bewege dich, um den Timer zurückzusetzen.</color>";

        [Description("Whether staff members with Remote Admin access receive a private broadcast when an SCP is moved to Spectator.")]
        public bool NotifyAdminsWhenScpMoved { get; set; } = true;

        [Description("Duration of the private admin broadcast in seconds.")]
        public ushort AdminNotificationDuration { get; set; } = 10;

        [Description("Private message shown to staff. Available placeholders: {player}, {userid}, {role}.")]
        public string AdminScpMovedMessage { get; set; } = "<color=#5B8CFF>[AFK]</color> <color=white>{player} ({role}) wurde wegen Inaktivität zum Zuschauer verschoben.</color>";
    }
}