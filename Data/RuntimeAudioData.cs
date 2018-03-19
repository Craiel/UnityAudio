using AudioChannel = Craiel.UnityAudio.Enums.AudioChannel;
using AudioFlags = Craiel.UnityAudio.Enums.AudioFlags;
using AudioPlayBehavior = Craiel.UnityAudio.Enums.AudioPlayBehavior;
using ResourceKey = Craiel.UnityEssentials.Resource.ResourceKey;

namespace Craiel.UnityAudio.Data
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityGameData;

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
