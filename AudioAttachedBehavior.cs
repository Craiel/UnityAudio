namespace Craiel.UnityAudio
{
    using UnityEngine;
    using UnityEssentials.Event;
    using UnityGameData;
    using UnityGameData.Events;

    public abstract class AudioAttachedBehavior : MonoBehaviour
    {
        private BaseEventSubscriptionTicket loadEventTicket;
        private BaseEventSubscriptionTicket unloadEventTicket;

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public bool DataLoaded { get; private set; }

        public virtual void Start()
        {
            this.loadEventTicket = GameEvents.Instance.Subscribe<EventGameDataLoaded>(this.OnGameDataLoaded);
            this.unloadEventTicket = GameEvents.Instance.Subscribe<EventGameDataUnloaded>(this.OnGameDataUnloaded);

            if (GameRuntimeData.Instance.IsLoaded)
            {
                // Data is already loaded so call setup immediatly
                // We keep the event ticket in case the data gets reloaded
                this.SetupAudio();
                this.DataLoaded = true;
            }
        }

        public virtual void OnDestroy()
        {
            GameEvents.Instance.Unsubscribe(ref this.loadEventTicket);
            GameEvents.Instance.Unsubscribe(ref this.unloadEventTicket);

            this.StopAllAudio();
        }

        public abstract void StopAllAudio();

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected virtual void SetupAudio()
        {
            this.StopAllAudio();
        }

        protected virtual void ReleaseAudio()
        {
            this.StopAllAudio();
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void OnGameDataLoaded(EventGameDataLoaded eventdata)
        {
            this.SetupAudio();
            this.DataLoaded = true;
        }

        private void OnGameDataUnloaded(EventGameDataUnloaded eventdata)
        {
            this.ReleaseAudio();
            this.DataLoaded = false;
        }
    }
}
