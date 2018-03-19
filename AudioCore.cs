using ResourceKey = Craiel.UnityEssentials.Resource.ResourceKey;

namespace Craiel.UnityAudio
{
    using Contracts;
    using UnityEssentials.Component;

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
