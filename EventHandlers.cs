using AfkManager.Features;
using Exiled.Events.EventArgs.Player;

namespace AfkManager
{
    internal sealed class EventHandlers
    {
        private readonly AfkService service;

        public EventHandlers(AfkService service)
        {
            this.service = service;
        }

        public void OnVerified(VerifiedEventArgs ev) => service.Reset(ev.Player);

        public void OnSpawned(SpawnedEventArgs ev) => service.Reset(ev.Player);

        public void OnLeft(LeftEventArgs ev) => service.Remove(ev.Player);

        public void OnRoundStarted()
        {
            service.Start();

            foreach (Exiled.API.Features.Player player in Exiled.API.Features.Player.List)
                service.Reset(player);
        }

        public void OnRoundEnded(Exiled.Events.EventArgs.Server.RoundEndedEventArgs ev) => service.Stop();

        public void OnRestartingRound() => service.Stop();
    }
}
