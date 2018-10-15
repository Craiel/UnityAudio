namespace Craiel.UnityAudio.Runtime
{
    using UnityEngine;
    using UnityEssentials.Runtime.Event;
    using UnityGameData;
    using UnityGameData.Runtime;
    using UnityGameData.Runtime.Events;

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
            GameEvents.Subscribe<EventGameDataLoaded>(this.OnGameDataLoaded, out this.loadEventTicket);
            GameEvents.Subscribe<EventGameDataUnloaded>(this.OnGameDataUnloaded, out this.unloadEventTicket);

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
            GameEvents.Unsubscribe(ref this.loadEventTicket);
            GameEvents.Unsubscribe(ref this.unloadEventTicket);

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
        private void OnGameDataLoaded(EventGameDataLoaded eventData)
        {
            this.SetupAudio();
            this.DataLoaded = true;
        }

        private void OnGameDataUnloaded(EventGameDataUnloaded eventData)
        {
            this.ReleaseAudio();
            this.DataLoaded = false;
        }
    }
}
