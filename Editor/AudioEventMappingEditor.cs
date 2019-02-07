namespace Craiel.UnityAudio.Editor
{
    using Runtime.Data;
    using Runtime.Enums;
    using UnityEditor;
    using UnityEngine;
    using UnityEssentials.Editor.UserInterface;
    using UnityGameData.Editor;

    [CustomEditor(typeof(AudioEventMapping))]
    public class AudioEventMappingEditor : Editor
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public override void OnInspectorGUI()
        {
            this.serializedObject.Update();
            this.Draw();
            this.serializedObject.ApplyModifiedProperties();
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void Draw()
        {
            SerializedProperty mappingProperty = this.serializedObject.FindProperty<AudioEventMapping>(x => x.Mapping);
            mappingProperty.arraySize = AudioEnumValues.AudioEventValues.Count;

            for (var i = 0; i < mappingProperty.arraySize; i++)
            {
                SerializedProperty element = mappingProperty.GetArrayElementAtIndex(i);
                
                AudioEvent eventValue = (AudioEvent) i;
                if (eventValue == AudioEvent.Unknown)
                {
                    continue;
                }
                
                var fakeRefHolder = CreateInstance<GameDataRefVirtualHolder>();
                var fakeRef = new GameDataAudioRef
                {
                    RefGuid = element.stringValue
                };

                fakeRefHolder.Ref = fakeRef;
                
                var fakeRefObj = new SerializedObject(fakeRefHolder);
                var prop = fakeRefObj.FindProperty<GameDataRefVirtualHolder>(x => x.Ref);
                EditorGUILayout.PropertyField(prop, new GUIContent(eventValue.ToString()));

                element.stringValue = fakeRef.RefGuid;
            }
        }
    }
}