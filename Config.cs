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

        [Description("Warning broadcast duration in seconds.")]
        public ushort WarningDuration { get; set; } = 10;

        [Description("Message shown after the warning timeout.")]
        public string WarningMessage { get; set; } = "<color=yellow>⚠ Du bist AFK. Bewege dich, sonst wirst du zum Zuschauer verschoben.</color>";

        [Description("Message shown immediately before moving the player to Spectator.")]
        public string MovedMessage { get; set; } = "<color=red>Du wurdest wegen Inaktivität zum Zuschauer verschoben.</color>";

        [Description("Whether staff members with Remote Admin access receive a private broadcast when an SCP is moved to Spectator.")]
        public bool NotifyAdminsWhenScpMoved { get; set; } = true;

        [Description("Duration of the private admin broadcast in seconds.")]
        public ushort AdminNotificationDuration { get; set; } = 10;

        [Description("Private message shown to staff. Available placeholders: {player}, {userid}, {role}.")]
        public string AdminScpMovedMessage { get; set; } = "<color=#ff5555>[AFK]</color> <color=white>{player} ({role}) wurde wegen Inaktivität zum Zuschauer verschoben.</color>";
    }
}