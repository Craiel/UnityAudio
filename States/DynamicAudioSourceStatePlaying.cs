namespace Craiel.UnityAudio.States
{
    using Assets.Scripts.Craiel.Audio.Enums;
    using Enums;

    public class DynamicAudioSourceStatePlaying : DynamicAudioSourceStateBase
    {
        public static readonly DynamicAudioSourceStatePlaying Instance = new DynamicAudioSourceStatePlaying();

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public override void Update(DynamicAudioSource entity)
        {
            base.Update(entity);

            if (!entity.Source.isPlaying)
            {
                entity.SwitchState(DynamicAudioSourceState.Finished);
            }
        }
    }
}
