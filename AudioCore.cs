namespace Assets.Scripts.Craiel.Essentials
{
    using System;
    using System.Linq;
    using Audio.Contracts;
    using Audio.Data;
    using GameData;
    using GameData.Contracts;
    using NLog;
    using Resource;

    public static class AudioCore
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();
        
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        static AudioCore()
        {
            Initialize();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public static ResourceKey MasterMixerResource { get; set; }
        public static ResourceKey DynamicAudioSourceResource { get; set; }
        
        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private static void Initialize()
        {
            Type configType = typeof(IAudioConfig);
            var implementations = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(x => x.GetTypes())
                .Where(x => !x.IsInterface && configType.IsAssignableFrom(x))
                .ToList();

            if (implementations.Count != 1)
            {
                Logger.Error("No implementation of IAudioConfig found, configure your game data first");
                return;
            }

            var config = Activator.CreateInstance(implementations.First()) as IAudioConfig;
            if (config == null)
            {
                Logger.Error("Failed to instantiate config class");
                return;
            }

            config.Configure();
        }
    }
}
