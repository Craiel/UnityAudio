using Telegram = Craiel.UnityEssentials.Msg.Telegram;

namespace Assets.Scripts.Craiel.Audio.States
{
    using Audio;

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
