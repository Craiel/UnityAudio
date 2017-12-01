namespace Assets.Scripts.Craiel.Audio.UI
{
    using Audio;
    using GameData;
    using UnityEngine;

    public abstract class UIAudioHelperBase : MonoBehaviour
    {
        private GameDataId audioId;

        private AudioTicket audioTicket;

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        [SerializeField]
        public GameDataRuntimeAudioRef Audio;

        public virtual void Awake()
        {
            this.audioId = GameRuntimeData.Instance.GetRuntimeId(this.Audio);
        }

        public virtual void OnDestroy()
        {
            this.StopAudio();
        }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected void PlayAudio()
        {
            this.StopAudio();
            if (this.audioId != GameDataId.Invalid)
            {
                this.audioTicket = AudioSystem.Instance.Play(this.audioId);
            }
        }

        protected void StopAudio()
        {
            if (this.audioTicket != AudioTicket.Invalid)
            {
                AudioSystem.Instance.Stop(this.audioTicket);
                this.audioTicket = AudioTicket.Invalid;
            }
        }
    }
}
