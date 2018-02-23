namespace Assets.Scripts.Craiel.Audio.Editor
{
    using Craiel.Essentials.Editor.UserInterface;
    using Craiel.GameData.Editor.Common;
    using Essentials.Editor.ReorderableList;
    using GameData.Editor;
    using GameData.Editor.Enums;
    using UnityEditor;
    using UnityEngine;

    [CustomEditor(typeof(GameDataAudio))]
    [CanEditMultipleObjects]
    public class GameDataAudioEditor : GameDataObjectEditor
    {
        private static bool propertiesFoldout = true;

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public override void DrawCompact()
        {
            base.DrawCompact();
            
            var typedTarget = (GameDataAudio)this.target;
            GUILayout.Label(string.Format(" Channel: {0}", typedTarget.AudioChannel));
        }
        
        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected override void DrawFull()
        {
            base.DrawFull();

            this.DrawProperties();
        }
        
        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void DrawProperties()
        {
            if (this.DrawFoldout("Audio Properties", ref propertiesFoldout))
            {
                EditorGUILayout.PropertyField(this.serializedObject.FindProperty<GameDataAudio>(x => x.AudioChannel), true);
                EditorGUILayout.PropertyField(this.serializedObject.FindProperty<GameDataAudio>(x => x.PlayBehavior), true);
                EditorGUILayout.PropertyField(this.serializedObject.FindProperty<GameDataAudio>(x => x.OnlyOneAtATime), true);

                if (!this.serializedObject.isEditingMultipleObjects)
                {
                    ReorderableListGUI.Title("Clips");
                    ReorderableListGUI.ListField(this.serializedObject.FindProperty<GameDataAudio>(x => x.AudioClips));
                }
            }
        }
    }
}