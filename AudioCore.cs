using ResourceKey = Craiel.UnityEssentials.Resource.ResourceKey;

namespace Assets.Scripts.Craiel.Audio
{
    using Contracts;

    public static class AudioCore
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        static AudioCore()
        {
            new CraielComponentConfigurator<IAudioConfig>().Configure();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public static ResourceKey MasterMixerResource { get; set; }
        public static ResourceKey DynamicAudioSourceResource { get; set; }
    }
}
