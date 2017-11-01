namespace Assets.Scripts.Craiel.Audio.States
{
    using Audio;
    using Craiel.GDX.AI.Sharp.Contracts;
    using Craiel.GDX.AI.Sharp.Msg;

    public class DynamicAudioSourceStateBase : IState<DynamicAudioSource>
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public virtual void Enter(DynamicAudioSource entity)
        {
        }

        public virtual void Update(DynamicAudioSource entity)
        {
        }

        public void Exit(DynamicAudioSource entity)
        {
        }

        public bool OnMessage(DynamicAudioSource entity, Telegram telegram)
        {
            return false;
        }
    }
}
