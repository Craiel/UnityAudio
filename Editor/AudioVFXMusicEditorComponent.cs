﻿using IVFXEditorComponent = Craiel.GameData.Editor.Contracts.VFXShared.IVFXEditorComponent;

namespace Assets.Scripts.Craiel.Audio.Editor
{
    public class AudioVFXMusicEditorComponent : IVFXEditorComponent
    {
        public string Category
        {
            get { return AudioEditorConstants.VFXComponentCategory; }
        }

        public string Name
        {
            get { return "Music"; }
        }
    }
}