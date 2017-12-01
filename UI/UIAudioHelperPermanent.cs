namespace Assets.Scripts.Craiel.Audio.UI
{
    public class UIAudioHelperPermanent : UIAudioHelperBase
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public override void Awake()
        {
            base.Awake();

            this.PlayAudio();
        }
    }
}
