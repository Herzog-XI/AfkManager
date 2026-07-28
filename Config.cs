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

        [Description("Persistent AFK warning. Available placeholders: {time}, {bar}, {color}.")]
        public string WarningMessage { get; set; } = "<color=#4F7DFF><b>AFK-Erkennung</b></color>\n<color=#FFD966>Du bist seit längerer Zeit inaktiv.</color>\n<size=30><color={color}><b>{time}</b></color></size>\n<color={color}>{bar}</color>\n<size=18><color=#CFCFCF>Bewege dich, um den Timer zurückzusetzen.</color></size>";

        [Description("Number of characters used for the countdown progress bar.")]
        public int ProgressBarLength { get; set; } = 16;

        [Description("Message shown after moving the player to Spectator.")]
        public string MovedMessage { get; set; } = "<color=#FF5A5A><b>Du wurdest wegen Inaktivität zum Zuschauer verschoben.</b></color>\n<size=18><color=#BEBEBE>Du kannst der nächsten Runde wieder normal beitreten.</color></size>";

        [Description("Duration of the moved-to-Spectator message in seconds.")]
        public ushort MovedMessageDuration { get; set; } = 8;

        [Description("Whether staff members with Remote Admin access receive a private broadcast when an SCP is moved to Spectator.")]
        public bool NotifyAdminsWhenScpMoved { get; set; } = true;

        [Description("Duration of the private admin broadcast in seconds.")]
        public ushort AdminNotificationDuration { get; set; } = 10;

        [Description("Private message shown to staff. Available placeholders: {player}, {userid}, {role}.")]
        public string AdminScpMovedMessage { get; set; } = "<color=#FF5A5A>[AFK]</color> <color=white>{player} ({role}) wurde wegen Inaktivität zum Zuschauer verschoben.</color>";
    }
}
