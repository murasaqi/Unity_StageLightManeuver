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
        private SerializedProperty clockProperty;
        
        private void OnEnable()
        {
            clockProperty = serializedObject.FindProperty("clockProperty");
        }
        
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            EditorGUILayout.LabelField("StageLightMasterClock Settings", EditorStyles.boldLabel);
            EditorGUILayout.Space();
            
            // ClockPropertyを表示（StageLightMasterClockPropertyDrawerが使用される）
            EditorGUILayout.PropertyField(clockProperty);
            
            serializedObject.ApplyModifiedProperties();
        }
    }
}