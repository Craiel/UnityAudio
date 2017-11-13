namespace Assets.Scripts.Craiel.Essentials
{
    using Audio.Contracts;
    using Resource;
    using UnityComponent;

    public static class AudioCore
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        static AudioCore()
        {
            new UnityComponentConfigurator<IAudioConfig>().Configure();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public static ResourceKey MasterMixerResource { get; set; }
        public static ResourceKey DynamicAudioSourceResource { get; set; }
    }
}
