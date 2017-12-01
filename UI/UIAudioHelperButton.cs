namespace Assets.Scripts.Craiel.Audio.UI
{
    using UnityEngine;
    using UnityEngine.UI;

    public class UIAudioHelperButton : UIAudioHelperBase
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        [SerializeField]
        public Button Target;

        public override void Awake()
        {
            base.Awake();

            this.Target.onClick.AddListener(this.OnTargetClick);
        }

        public override void OnDestroy()
        {
            this.Target.onClick.RemoveListener(this.OnTargetClick);

            base.OnDestroy();
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void OnTargetClick()
        {
            this.PlayAudio();
        }
    }
}
