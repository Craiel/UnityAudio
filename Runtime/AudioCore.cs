using ResourceKey = Craiel.UnityEssentials.Runtime.Resource.ResourceKey;

namespace Craiel.UnityAudio.Runtime
{
    using Contracts;
    using UnityEssentials.Runtime.Component;

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
