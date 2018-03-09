using IVFXEditorComponent = Craiel.GameData.Editor.Contracts.VFXShared.IVFXEditorComponent;
using IVFXEditorComponentFactory = Craiel.GameData.Editor.Contracts.VFXShared.IVFXEditorComponentFactory;

namespace Assets.Scripts.Craiel.Audio.Editor
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using VFX.Editor.Components;

    public class AudioVFXEditorComponentFactory : IVFXEditorComponentFactory
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public AudioVFXEditorComponentFactory()
        {
            this.AvailableComponents = new List<VFXEditorComponentDescriptor>
            {
                new VFXEditorComponentDescriptor(this)
                {
                    Type = typeof(AudioVFXMusicEditorComponent),
                    Category = AudioEditorConstants.VFXComponentCategory,
                    Name = "Music"
                },
                new VFXEditorComponentDescriptor(this)
                {
                    Type = typeof(AudioVFXSFXEditorComponent),
                    Category = AudioEditorConstants.VFXComponentCategory,
                    Name = "SFX"
                }
            };
        }
        
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public IList<VFXEditorComponentDescriptor> AvailableComponents { get; private set; }
        
        public IVFXEditorComponent CreateNew(VFXEditorComponentDescriptor descriptor, Vector2 position)
        {
            var entry = (IVFXEditorComponent) System.Activator.CreateInstance(descriptor.Type);
            entry.Position = position;
            entry.Name = string.Format("New {0}", descriptor.Name);
            
            return entry;
        }
    }
}