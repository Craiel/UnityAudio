﻿namespace Craiel.UnityAudio.Runtime
{
    using System.Collections.Generic;
    using Data;
    using Enums;
    using NLog;
    using UnityEngine;
    using UnityEngine.Audio;
    using UnityEssentials.Runtime.Enums;
    using UnityEssentials.Runtime.Scene;
    using UnityEssentials.Runtime.Singletons;
    using UnityGameData;
    using UnityGameData.Runtime;

    public class AudioSystem : UnitySingletonBehavior<AudioSystem>
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly DynamicAudioSourcePool dynamicAudioSourcePool;
        
        private readonly IDictionary<AudioTicket, DynamicAudioSource> activeSources;
        
        private readonly IDictionary<GameDataId, IList<AudioTicket>> sourcesByDataMap;
        
        private AudioMixerController masterMixer;
        
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public AudioSystem()
        {
            this.dynamicAudioSourcePool = new DynamicAudioSourcePool();
            this.activeSources = new Dictionary<AudioTicket, DynamicAudioSource>();
            this.sourcesByDataMap = new Dictionary<GameDataId, IList<AudioTicket>>();
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
            
            AudioAreaSystem.InstantiateAndInitialize();

            Logger.Info("Audio Manager Initialized");
        }

        public void LateUpdate()
        {
            this.dynamicAudioSourcePool.Update();
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
                    Logger.Warn("Invalid audio channel in clip {0}: {1}", entry.Id, entry.Channel);
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
                    Logger.Warn("Invalid audio channel in clip {0}: {1}", entry.Id, entry.Channel);
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
                    Logger.Warn("Invalid audio channel in clip {0}: {1}", entry.Id, entry.Channel);
                    return AudioTicket.Invalid;
                }

                var ticket = AudioTicket.Next();
                source.SetPosition(position);
                source.Play(ticket, entry, true, channel, parameters);

                this.RegisterSource(ticket, source);
                return ticket;
            }

            return AudioTicket.Invalid;
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