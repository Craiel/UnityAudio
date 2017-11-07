namespace Assets.Scripts.Craiel.Audio
{
    using Assets.Scripts.Craiel.GameData;
    using Essentials;
    using UnityEngine;

    public class AudioAreaEmitter : MonoBehaviour
    {
        private AudioTicket audioTicket;

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        [SerializeField]
        public GameObject Target;

        [SerializeField]
        public GameDataRuntimeAudioRef Audio;

        [SerializeField]
        public bool IsLocalized;

        [SerializeField]
        public bool ActivateOnTrigger = true;

        [SerializeField]
        public float FadeIn = 1.0f;

        [SerializeField]
        public float FadeOut = 1.0f;

        [SerializeField]
        public int Priority;

        [SerializeField]
        public bool IgnorePriority;

        public GameDataId AudioId { get; private set; }

        public void Update()
        {
            if (this.Audio != null && (this.AudioId == GameDataId.Invalid || this.AudioId.Guid != this.Audio.RefGuid))
            {
                this.AudioId = AudioCore.GameDataRuntimeResolver.GetRuntimeId(this.Audio);
            }

            if (!this.ActivateOnTrigger)
            {
                this.Activate();
            }
        }

        public void OnDestroy()
        {
            this.Deactivate();
        }

        public void OnTriggerEnter(Collider triggerCollider)
        {
            if (this.Target != null && triggerCollider.gameObject != this.Target)
            {
                return;
            }

            this.Activate();
        }

        public void OnTriggerExit(Collider triggerCollider)
        {
            if (this.Target != null && triggerCollider.gameObject != this.Target)
            {
                return;
            }

            this.Deactivate();
        }

        public void Play()
        {
            this.Stop();

            var parameters = new AudioPlayParameters
            {
                FadeIn = this.FadeIn,
                FadeOut = this.FadeOut
            };

            if (this.IsLocalized)
            {
                this.audioTicket = AudioSystem.Instance.PlayStationary(this.gameObject.transform.position, this.AudioId, parameters);
            }
            else
            {
                this.audioTicket = AudioSystem.Instance.Play(this.AudioId, parameters);
            }
        }

        public void Stop()
        {
            if (this.audioTicket == AudioTicket.Invalid)
            {
                return;
            }

            AudioSystem.Instance.Stop(this.audioTicket);
            this.audioTicket = AudioTicket.Invalid;
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void Activate()
        {
            if (this.Audio == null)
            {
                return;
            }

            AudioAreaSystem.Instance.Activate(this);
        }

        private void Deactivate()
        {
            if (this.Audio == null)
            {
                return;
            }

            AudioAreaSystem.Instance.Deactivate(this);
        }
    }
}