using UnityEditor;
using UnityEngine;

namespace StageLightManeuver
{
    /// <summary>
    /// StageLightMasterClockClipのカスタムエディタ
    /// </summary>
    [CustomEditor(typeof(StageLightMasterClockClip))]
    public class StageLightMasterClockClipEditor : Editor
    {
        private SerializedProperty clockSettings;
        private SerializedProperty bpm;
        private SerializedProperty bpmScale;
        
        private void OnEnable()
        {
            clockSettings = serializedObject.FindProperty("clockSettings");
            bpm = clockSettings.FindPropertyRelative("bpm");
            bpmScale = clockSettings.FindPropertyRelative("bpmScale");
        }
        
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            EditorGUILayout.LabelField("StageLightMasterClock Settings", EditorStyles.boldLabel);
            EditorGUILayout.Space();
            
            // BPMとBPM Scaleのみを表示
            EditorGUILayout.PropertyField(bpm, new GUIContent("BPM", "Beats Per Minute"));
            EditorGUILayout.PropertyField(bpmScale, new GUIContent("BPM Scale", "Scale factor for BPM"));
            
            serializedObject.ApplyModifiedProperties();
        }
    }
}