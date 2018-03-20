﻿namespace Craiel.UnityAudio.Enums
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class AudioEnumValues
    {
        // -------------------------------------------------------------------
        // Auto Generated by AudioEnumValues.tt DO NOT MODIFY DIRECTLY!
        // -------------------------------------------------------------------
        public static readonly IList<AudioChannel> AudioChannelValues = Enum.GetValues(typeof(AudioChannel)).Cast<AudioChannel>().ToList();

        public static readonly IList<AudioFlags> AudioFlagsValues = Enum.GetValues(typeof(AudioFlags)).Cast<AudioFlags>().ToList();

        public static readonly IList<AudioPlayBehavior> AudioPlayBehaviorValues = Enum.GetValues(typeof(AudioPlayBehavior)).Cast<AudioPlayBehavior>().ToList();

        public static readonly IList<DynamicAudioSourceState> DynamicAudioSourceStateValues = Enum.GetValues(typeof(DynamicAudioSourceState)).Cast<DynamicAudioSourceState>().ToList();

    }
}
