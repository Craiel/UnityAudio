namespace Assets.Scripts.Craiel.Audio.States
{
    using Audio;

    public class DynamicAudioSourceStateInactive : DynamicAudioSourceStateBase
    {
        public static readonly DynamicAudioSourceStateInactive Instance = new DynamicAudioSourceStateInactive();

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public override void Update(DynamicAudioSource entity)
        {
            base.Update(entity);
        }
    }
}
