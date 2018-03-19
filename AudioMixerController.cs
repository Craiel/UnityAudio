using ResourceKey = Craiel.UnityEssentials.Resource.ResourceKey;
using ResourceProvider = Craiel.UnityEssentials.Resource.ResourceProvider;

namespace Craiel.UnityAudio
{
    using System;
    using System.Collections.Generic;
    using Assets.Scripts.Craiel.Audio.Enums;
    using Enums;
    using UnityEngine.Audio;

    public class AudioMixerController
    {
        private readonly AudioMixer mixer;

        private readonly IDictionary<string, AudioMixerGroup> groups;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        private AudioMixerController()
        {
            this.groups = new Dictionary<string, AudioMixerGroup>();
            this.Channels = new List<AudioMixerGroup>();
        }

        public AudioMixerController(AudioMixer mixer)
            : this()
        {
            this.mixer = mixer;

            this.Reload();
        }

        public AudioMixerController(ResourceKey mixerKey)
            : this()
        {
            using (var resource = ResourceProvider.Instance.AcquireOrLoadResource<AudioMixer>(mixerKey))
            {
                if (resource != null && resource.Data != null)
                {
                    this.mixer = resource.Data;
                }
            }

            if (this.mixer == null)
            {
                throw new InvalidOperationException("Could not load mixer from Resource Key: " + mixerKey);
            }

            this.UpdateMixerProperties();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public IList<AudioMixerGroup> Channels { get; private set; }

        public AudioMixerGroup GetChannel(AudioChannel channel)
        {
            return this.GetChannel(channel.ToString());
        }

        public AudioMixerGroup GetChannel(string channel)
        {
            AudioMixerGroup mixerGroup;
            if (this.groups.TryGetValue(channel.ToLowerInvariant(), out mixerGroup))
            {
                return mixerGroup;
            }

            return null;
        }

        public void Reload()
        {
            this.UpdateMixerProperties();
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void UpdateMixerProperties()
        {
            this.groups.Clear();
            this.Channels.Clear();

            AudioMixerGroup[] matchingGroups = this.mixer.FindMatchingGroups("Master");
            foreach (AudioMixerGroup mixerGroup in matchingGroups)
            {
                this.groups.Add(mixerGroup.name.ToLowerInvariant(), mixerGroup);
                this.Channels.Add(mixerGroup);
            }
        }
    }
}