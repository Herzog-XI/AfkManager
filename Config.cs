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

        [Description("Seconds of inactivity before the warning is shown.")]
        public float WarningAfter { get; set; } = 60f;

        [Description("Seconds of inactivity before the player is moved to Spectator.")]
        public float MoveAfter { get; set; } = 120f;

        [Description("How often player activity is checked, in seconds.")]
        public float CheckInterval { get; set; } = 1f;

        [Description("Minimum movement distance that counts as activity.")]
        public float MovementThreshold { get; set; } = 0.05f;

        [Description("Minimum camera rotation in degrees that counts as activity.")]
        public float RotationThreshold { get; set; } = 1f;

        [Description("Warning broadcast duration in seconds.")]
        public ushort WarningDuration { get; set; } = 10;

        [Description("Message shown after the warning timeout.")]
        public string WarningMessage { get; set; } = "<color=yellow>⚠ Du bist seit einer Minute AFK. Bewege dich innerhalb der nächsten Minute, sonst wirst du zum Zuschauer verschoben.</color>";

        [Description("Message shown immediately before moving the player to Spectator.")]
        public string MovedMessage { get; set; } = "<color=red>Du wurdest wegen Inaktivität zum Zuschauer verschoben.</color>";
    }
}
