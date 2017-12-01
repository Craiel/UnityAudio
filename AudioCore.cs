namespace Assets.Scripts.Craiel.Audio
{
    using Contracts;
    using Essentials.Component;
    using Essentials.Resource;

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
