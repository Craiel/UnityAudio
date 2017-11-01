namespace Assets.Scripts.Craiel.Audio
{
    using System.Collections.Generic;
    using Craiel.Essentials;
    using Craiel.Essentials.Enums;
    using Craiel.Essentials.Resource;
    using Craiel.Essentials.Scene;
    using Craiel.GameData;
    using Data;
    using Enums;
    using NLog;
    using UnityEngine;
    using UnityEngine.Audio;

    public class AudioSystem : UnitySingletonBehavior<AudioSystem>
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        private static uint nextTicketId = 1;

        private readonly DynamicAudioSourcePool dynamicAudioSourcePool;

        private readonly IDictionary<AudioChannel, AudioMixerGroup> channelGroupLookup;

        private readonly IDictionary<AudioTicket, DynamicAudioSource> activeSources;
        
        private readonly IDictionary<GameDataId, IList<AudioTicket>> sourcesByDataMap;
        
        private AudioMixer masterMixer;

        private SceneObjectRoot audioSourceRoot;
        
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public AudioSystem()
        {
            this.dynamicAudioSourcePool = new DynamicAudioSourcePool();
            this.channelGroupLookup = new Dictionary<AudioChannel, AudioMixerGroup>();
            this.activeSources = new Dictionary<AudioTicket, DynamicAudioSource>();
            this.sourcesByDataMap = new Dictionary<GameDataId, IList<AudioTicket>>();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public override void Awake()
        {
            this.RegisterInController(SceneObjectController.Instance, SceneRootCategory.System, true);

            base.Awake();

            this.audioSourceRoot = SceneObjectController.Instance.AcquireRoot(SceneRootCategory.Dynamic, "DynamicAudioSource", true);
        }

        public override void Initialize()
        {
            base.Initialize();

            using (var resource = ResourceProvider.Instance.AcquireOrLoadResource<AudioMixer>(AssetResourceKeys.MasterMixerResourceKey))
            {
                this.masterMixer = resource.Data;
                foreach (AudioChannel channel in AudioEnumValues.AudioChannelValues)
                {
                    if (channel == AudioChannel.Unknown)
                    {
                        continue;
                    }

                    AudioMixerGroup[] channelGroups = this.masterMixer.FindMatchingGroups(channel.ToString());
                    if (channelGroups.Length != 1)
                    {
                        Logger.Error("Missing or ambiguous Sound Channel: {0}", channel);
                        continue;
                    }

                    this.channelGroupLookup.Add(channel, channelGroups[0]);
                }
            }

            using (var resource = ResourceProvider.Instance.AcquireOrLoadResource<GameObject>(AssetResourceKeys.DynamicAudioSourcePrefabResourceKey))
            {
                if (resource == null || resource.Data == null)
                {
                    Logger.Error("Missing Dynamic Audio Source Prefab: {0}", AssetResourceKeys.DynamicAudioSourcePrefabResourceKey);
                    return;
                }

                this.dynamicAudioSourcePool.Initialize(resource.Data, this.UpdateAudioSource, this.audioSourceRoot.GetTransform());
            }
            
            Logger.Info("Audio Manager Initialized");
        }

        public void LateUpdate()
        {
            this.dynamicAudioSourcePool.Update();
        }

        public AudioTicket Play(GameDataId id, AudioPlayParameters parameters = default (AudioPlayParameters))
        {
            RuntimeAudioData entry = GameRuntimeData.Instance.Get<RuntimeAudioData>(id);
            if (entry == null)
            {
                Logger.Error("Could not play audio {0}, data not found", id);
                return AudioTicket.Invalid;
            }

            DynamicAudioSource source = this.PrepareAudioSource(entry);
            if (source == null)
            {
                return AudioTicket.Invalid;
            }

            AudioMixerGroup group;
            if (!this.channelGroupLookup.TryGetValue(entry.Channel, out group))
            {
                Logger.Warn("Invalid audio channel in clip {0}: {1}", entry.Id, entry.Channel);
                return AudioTicket.Invalid;
            }

            var ticket = new AudioTicket(nextTicketId++);
            source.Play(ticket, entry, false, group, parameters);

            this.RegisterSource(ticket, source);
            return ticket;
        }
        
        public void Stop(AudioTicket ticket)
        {
            DynamicAudioSource source;
            if (this.activeSources.TryGetValue(ticket, out source))
            {
                source.Stop();
            }
        }

        public void StopByDataId(GameDataId id)
        {
            IList<AudioTicket> tickets;
            if (this.sourcesByDataMap.TryGetValue(id, out tickets))
            {
                foreach (AudioTicket ticket in tickets)
                {
                    this.Stop(ticket);
                }
            }
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private DynamicAudioSource PrepareAudioSource(RuntimeAudioData entry)
        {
            if ((entry.Flags & AudioFlags.Unique) != 0)
            {
                if (this.sourcesByDataMap.ContainsKey(entry.Id))
                {
                    // Same audio is already playing and unique
                    return null;
                }
            }

            DynamicAudioSource source = this.dynamicAudioSourcePool.Obtain();
            source.gameObject.SetActive(true);
            return source;
        }
        
        private void RegisterSource(AudioTicket ticket, DynamicAudioSource source)
        {
            this.activeSources.Add(ticket, source);

            IList<AudioTicket> ticketList;
            if (!this.sourcesByDataMap.TryGetValue(source.ActiveId, out ticketList))
            {
                ticketList = new List<AudioTicket>();
                this.sourcesByDataMap.Add(source.ActiveId, ticketList);
            }

            ticketList.Add(ticket);
        }

        private void UnregisterSource(DynamicAudioSource source)
        {
            IList<AudioTicket> ticketList;
            if (this.sourcesByDataMap.TryGetValue(source.ActiveId, out ticketList))
            {
                this.activeSources.Remove(source.Ticket);
                ticketList.Remove(source.Ticket);
                if (ticketList.Count == 0)
                {
                    this.sourcesByDataMap.Remove(source.ActiveId);
                }
            }
        }

        private bool UpdateAudioSource(DynamicAudioSource source)
        {
            if (source.IsActive)
            {
                return true;
            }

            this.UnregisterSource(source);
            return false;
        }
    }
}
