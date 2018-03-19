﻿using AudioChannel = Craiel.UnityAudio.Enums.AudioChannel;
using AudioPlayBehavior = Craiel.UnityAudio.Enums.AudioPlayBehavior;

namespace Craiel.UnityAudio.Editor
{
    using System.Collections.Generic;
    using Data;
    using UnityEngine;
    using UnityGameData.Editor.Builder;
    using UnityGameData.Editor.Common;

    public class GameDataAudio : GameDataObject
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        [SerializeField]
        public AudioChannel AudioChannel;

        [SerializeField]
        public AudioPlayBehavior PlayBehavior;

        [SerializeField]
        public bool OnlyOneAtATime;

        [SerializeField]
        [HideInInspector]
        public List<GameResourceAudioClipRef> AudioClips;

        public override void Validate(GameDataBuildValidationContext context)
        {
            base.Validate(context);

            if (this.AudioChannel == AudioChannel.Unknown)
            {
                context.Error(this, this, null, "Audio channel is not set");
            }
            
            GameResourceRefBase.ValidateRefList(this, this, this.AudioClips, context);
        }

        public override void Build(GameDataBuildContext context)
        {
            var runtime = new RuntimeAudioData
            {
                Channel = this.AudioChannel,
                PlayBehavior = this.PlayBehavior,
                OnlyOneAtATime = this.OnlyOneAtATime
            };

            foreach (GameResourceAudioClipRef clip in this.AudioClips)
            {
                if (clip.IsValid())
                {
                    runtime.Clips.Add(clip.GetPath());
                }
            }

            this.BuildBase(context, runtime);

            context.AddBuildResult(runtime);
        }
    }
}