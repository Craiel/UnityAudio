namespace Assets.Scripts.Craiel.Audio.States
{
    using Audio;

    public class DynamicAudioSourceStateFinished : DynamicAudioSourceStateBase
    {
        public static readonly DynamicAudioSourceStateFinished Instance = new DynamicAudioSourceStateFinished();

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public override void Update(DynamicAudioSource entity)
        {
            entity.Reset();
        }
    }
}
