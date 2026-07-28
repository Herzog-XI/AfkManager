# AfkManager

EXILED plugin for SCP: Secret Laboratory that warns inactive players and moves them to Spectator after a configurable timeout.

## Default behaviour

- Checks player activity once per second
- Warns after 60 seconds of inactivity
- Moves the player to Spectator after 120 seconds
- Movement and camera rotation reset the AFK timer
- Spectator and Overwatch are excluded automatically
- Newly added playable roles are monitored automatically

## Configuration

```yaml
is_enabled: true
debug: false
warning_after: 60
move_after: 120
check_interval: 1
movement_threshold: 0.05
rotation_threshold: 1
warning_duration: 10
warning_message: '<color=yellow>⚠ Du bist seit einer Minute AFK. Bewege dich innerhalb der nächsten Minute, sonst wirst du zum Zuschauer verschoben.</color>'
moved_message: '<color=red>Du wurdest wegen Inaktivität zum Zuschauer verschoben.</color>'
```

## Build

The project targets .NET Framework 4.8 and uses `ExMod.Exiled` 9.14.2. GitHub Actions builds the plugin automatically and uploads `AfkManager.dll` as an artifact.
