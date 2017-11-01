namespace Assets.Scripts.Craiel.Audio.UI.Utils
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
