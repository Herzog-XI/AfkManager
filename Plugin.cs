using System;
using AfkManager.Features;
using Exiled.API.Features;
using Player = Exiled.Events.Handlers.Player;
using Server = Exiled.Events.Handlers.Server;

namespace AfkManager
{
    public sealed class Plugin : Plugin<Config>
    {
        public static Plugin Instance { get; private set; }

        public override string Name => "AfkManager";
        public override string Author => "Herzog-XI";
        public override Version Version => new Version(1, 0, 0);
        public override Version RequiredExiledVersion => new Version(9, 14, 2);

        private AfkService service;
        private EventHandlers handlers;

        public override void OnEnabled()
        {
            Instance = this;
            service = new AfkService();
            handlers = new EventHandlers(service);

            Player.Verified += handlers.OnVerified;
            Player.Spawned += handlers.OnSpawned;
            Player.Left += handlers.OnLeft;
            Server.RoundStarted += handlers.OnRoundStarted;
            Server.RoundEnded += handlers.OnRoundEnded;
            Server.RestartingRound += handlers.OnRestartingRound;

            service.Start();
            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            Player.Verified -= handlers.OnVerified;
            Player.Spawned -= handlers.OnSpawned;
            Player.Left -= handlers.OnLeft;
            Server.RoundStarted -= handlers.OnRoundStarted;
            Server.RoundEnded -= handlers.OnRoundEnded;
            Server.RestartingRound -= handlers.OnRestartingRound;

            service?.Stop();
            handlers = null;
            service = null;
            Instance = null;

            base.OnDisabled();
        }
    }
}
