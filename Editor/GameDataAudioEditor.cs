namespace Assets.Scripts.Craiel.Audio.Editor
{
    using Craiel.Essentials.Editor.UserInterface;
    using Craiel.GameData.Editor.Common;
    using Essentials.Editor.ReorderableList;
    using GameData.Editor;
    using GameData.Editor.Enums;
    using UnityEditor;

    [CustomEditor(typeof(GameDataAudio))]
    [CanEditMultipleObjects]
    public class GameDataAudioEditor : GameDataObjectEditor
    {
        private static bool propertiesFoldout = true;

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public override void DrawGUI()
        {
            switch (GameDataEditorCore.Config.GetViewMode())
            {
                case GameDataEditorViewMode.Compact:
                {
                    this.DrawCompact();
                    break;
                }
                    
                case GameDataEditorViewMode.Full:
                {
                    this.DrawFull();
                    break;
                }
            }
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void DrawCompact()
        {
        }

        private void DrawFull()
        {
            this.DrawProperties();            
        }
        
        private void DrawProperties()
        {
            if (this.DrawFoldout("Properties", ref propertiesFoldout))
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