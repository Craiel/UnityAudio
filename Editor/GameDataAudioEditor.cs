namespace Assets.Scripts.Craiel.Audio.Editor
{
    using Craiel.Essentials.Editor.UserInterface;
    using Craiel.GameData.Editor.Common;
    using Essentials.Editor.ReorderableList;
    using UnityEditor;

    [CustomEditor(typeof(GameDataAudio))]
    [CanEditMultipleObjects]
    public class GameDataAudioEditor : GameDataObjectEditor
    {
        private static bool visualFoldout = true;

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public override void DrawGUI()
        {
            this.DrawProperties();
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void DrawProperties()
        {
            if (this.DrawFoldout("Properties", ref visualFoldout))
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