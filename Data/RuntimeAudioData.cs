using ResourceKey = Craiel.UnityEssentials.Resource.ResourceKey;

namespace Assets.Scripts.Craiel.Audio.Data
{
    using System;
    using System.Collections.Generic;
    using Craiel.GameData;
    using Enums;
    using UnityEngine;

    [Serializable]
    public class RuntimeAudioData : RuntimeGameData
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public RuntimeAudioData()
        {
            this.Clips = new List<string>();
            this.ClipKeys = new List<ResourceKey>();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        [SerializeField]
        public AudioChannel Channel;

        [SerializeField]
        public AudioPlayBehavior PlayBehavior;

        [SerializeField]
        public bool OnlyOneAtATime;

        [SerializeField]
        public AudioFlags Flags;

        [SerializeField]
        public List<string> Clips;

        public IList<ResourceKey> ClipKeys { get; private set; }

        public override void PostLoad()
        {
            base.PostLoad();

            foreach (string path in this.Clips)
            {
                this.ClipKeys.Add(ResourceKey.Create<AudioClip>(path));
            }
        }
    }
}
