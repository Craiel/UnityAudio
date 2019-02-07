namespace Craiel.UnityAudio.Runtime
{
    using System.Collections.Generic;
    using Data;
    using Enums;
    using UnityEngine;
    using UnityEngine.Audio;
    using UnityEssentials.Runtime.Enums;
    using UnityEssentials.Runtime.Resource;
    using UnityEssentials.Runtime.Scene;
    using UnityEssentials.Runtime.Singletons;
    using UnityGameData.Runtime;

    public class AudioSystem : UnitySingletonBehavior<AudioSystem>
    {
        private static readonly AudioPlayParameters DefaultPlayParameters = new AudioPlayParameters { UseRandomClip = true };
        
        private readonly DynamicAudioSourcePool dynamicAudioSourcePool;
        
        private readonly IDictionary<AudioTicket, DynamicAudioSource> activeSources;
        
        private readonly IDictionary<GameDataId, IList<AudioTicket>> sourcesByDataMap;
        
        private readonly IList<AudioTicket> managedAudioTickets;
        private readonly IList<AudioTicket> ticketTempList;
        
        private AudioMixerController masterMixer;

        private AudioEventMapping eventMapping;
        
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public AudioSystem()
        {
            this.dynamicAudioSourcePool = new DynamicAudioSourcePool();
            this.activeSources = new Dictionary<AudioTicket, DynamicAudioSource>();
            this.sourcesByDataMap = new Dictionary<GameDataId, IList<AudioTicket>>();
            this.managedAudioTickets = new List<AudioTicket>();
            this.ticketTempList = new List<AudioTicket>();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public override void Awake()
        {
            SceneObjectController.Instance.RegisterObjectAsRoot(SceneRootCategory.System, this.gameObject, true);

            base.Awake();
        }

        public override void Initialize()
        {
            base.Initialize();

            this.masterMixer = new AudioMixerController(AudioCore.MasterMixerResource);

            this.dynamicAudioSourcePool.Initialize(AudioCore.DynamicAudioSourceResource, this.UpdateAudioSource);

            this.eventMapping = this.LoadEventMapping();
            
            AudioAreaSystem.InstantiateAndInitialize();

            AudioCore.Logger.Info("Audio Manager Initialized");
        }

        public void LateUpdate()
        {
            this.UpdateManagedTickets();
            
            this.dynamicAudioSourcePool.Update();
        }
        
        public bool IsFinished(AudioTicket ticket)
        {
            DynamicAudioSource source;
            if (this.activeSources.TryGetValue(ticket, out source))
            {
                return !source.IsActive;
            }

            return true;
        }

        public AudioTicket Play(GameDataId id, AudioPlayParameters parameters = default (AudioPlayParameters))
        {
            var entry = GameRuntimeData.Instance.Get<RuntimeAudioData>(id);
            if (entry != null)
            {
                DynamicAudioSource source = this.PrepareAudioSource(entry);
                if (source == null)
                {
                    return AudioTicket.Invalid;
                }

                AudioMixerGroup group = this.masterMixer.GetChannel(entry.Channel);
                if (group == null)
                {
                    AudioCore.Logger.Warn("Invalid audio channel in clip {0}: {1}", entry.Id, entry.Channel);
                    return AudioTicket.Invalid;
                }

                var ticket = AudioTicket.Next();
                source.Play(ticket, entry, false, group, parameters);

                this.RegisterSource(ticket, source);
                return ticket;
            }

            return AudioTicket.Invalid;
        }

        public AudioTicket PlayAnchored(Transform anchorTransform, GameDataId id, AudioPlayParameters parameters = default(AudioPlayParameters))
        {
            var entry = GameRuntimeData.Instance.Get<RuntimeAudioData>(id);
            if (entry != null)
            {
                DynamicAudioSource source = this.PrepareAudioSource(entry);
                if (source == null)
                {
                    return AudioTicket.Invalid;
                }

                AudioMixerGroup channel = this.masterMixer.GetChannel(entry.Channel);
                if (channel == null)
                {
                    AudioCore.Logger.Warn("Invalid audio channel in clip {0}: {1}", entry.Id, entry.Channel);
                    return AudioTicket.Invalid;
                }

                var ticket = AudioTicket.Next();
                source.SetAnchor(anchorTransform);
                source.Play(ticket, entry, true, channel, parameters);

                this.RegisterSource(ticket, source);
                return ticket;
            }

            return AudioTicket.Invalid;
        }

        public AudioTicket PlayStationary(Vector3 position, GameDataId id, AudioPlayParameters parameters = default(AudioPlayParameters))
        {
            var entry = GameRuntimeData.Instance.Get<RuntimeAudioData>(id);
            if (entry == null)
            {
                return AudioTicket.Invalid;
            }
            
            DynamicAudioSource source = this.PrepareAudioSource(entry);
            if (source == null)
            {
                return AudioTicket.Invalid;
            }

            AudioMixerGroup channel = this.masterMixer.GetChannel(entry.Channel);
            if (channel == null)
            {
                AudioCore.Logger.Warn("Invalid audio channel in clip {0}: {1}", entry.Id, entry.Channel);
                return AudioTicket.Invalid;
            }

            var ticket = AudioTicket.Next();
            source.SetPosition(position);
            source.Play(ticket, entry, true, channel, parameters);

            this.RegisterSource(ticket, source);
            return ticket;
        }
        
        public void PlayAudioEventManaged(AudioEvent eventType)
        {
            this.PlayAudioEventManaged(eventType, DefaultPlayParameters);
        }
        
        public void PlayAudioEventManaged(AudioEvent eventType, AudioPlayParameters parameters)
        {
            AudioTicket ticket = this.PlayAudioEvent(eventType);
            if (ticket != null)
            {
                this.managedAudioTickets.Add(ticket);
            }
        }
        
        public AudioTicket PlayAudioEvent(AudioEvent eventType)
        {
            return this.PlayAudioEvent(eventType, DefaultPlayParameters);
        }

        public AudioTicket PlayAudioEvent(AudioEvent eventType, AudioPlayParameters parameters)
        {
            string audioDataGuid = this.eventMapping.Mapping[(int) eventType];
            if (string.IsNullOrEmpty(audioDataGuid))
            {
                AudioCore.Logger.Error("PlayAudioEvent Failed, No Guid Mapped for Event {0}", eventType);
                return AudioTicket.Invalid;
            }
            
            GameDataId audioDataId = GameRuntimeData.Instance.GetRuntimeId(audioDataGuid);
            if (audioDataId == GameDataId.Invalid)
            {
                AudioCore.Logger.Error("PlayAudioEvent Failed, DataId not found for {0}", audioDataGuid);
                return AudioTicket.Invalid;
            }

            return this.Play(audioDataId, parameters);
        }

        public void Stop(ref AudioTicket ticket)
        {
            DynamicAudioSource source;
            if (this.activeSources.TryGetValue(ticket, out source))
            {
                source.Stop();
            }

            ticket = AudioTicket.Invalid;
        }

        public void StopByDataId(GameDataId id)
        {
            IList<AudioTicket> tickets;
            if (this.sourcesByDataMap.TryGetValue(id, out tickets))
            {
                for (var i = 0; i < tickets.Count; i++)
                {
                    AudioTicket ticket = tickets[i];
                    this.Stop(ref ticket);
                }

                tickets.Clear();
                this.sourcesByDataMap.Remove(id);
            }
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void UpdateManagedTickets()
        {
            this.ticketTempList.Clear();
            for (var i = 0; i < this.managedAudioTickets.Count; i++)
            {
                AudioTicket ticket = this.managedAudioTickets[i];
                if (this.IsFinished(ticket))
                {
                    this.ticketTempList.Add(ticket);
                    this.Stop(ref ticket);
                }
            }

            for (var i = 0; i < this.ticketTempList.Count; i++)
            {
                this.managedAudioTickets.Remove(this.ticketTempList[i]);
            }

            this.ticketTempList.Clear();
        }
        
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
        
        private AudioEventMapping LoadEventMapping()
        {
            if (AudioCore.AudioEventMappingResource != ResourceKey.Invalid)
            {
                using (var eventDataResource = ResourceProvider.Instance.AcquireOrLoadResource<AudioEventMapping>(AudioCore.AudioEventMappingResource))
                {
                    if (eventDataResource.Data != null)
                    {
                        return eventDataResource.Data;
                    }
                }
            }

            return ScriptableObject.CreateInstance<AudioEventMapping>();
        }
    }
}
