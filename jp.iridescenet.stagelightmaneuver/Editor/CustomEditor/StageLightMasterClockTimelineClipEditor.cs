using UnityEditor;
using UnityEngine;

namespace StageLightManeuver
{
    /// <summary>
    /// ClockTimelineClip用のカスタムエディタ
    /// </summary>
    [CustomEditor(typeof(StageLightMasterClockTimelineClip))]
    public class StageLightMasterClockTimelineClipEditor : Editor
    {
        private SerializedProperty clockPropertyProp;
        
        private void OnEnable()
        {
            clockPropertyProp = serializedObject.FindProperty("clockProperty");
        }
        
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            EditorGUILayout.LabelField("Clock Settings", EditorStyles.boldLabel);
            
            // ClockPropertyの各プロパティを表示
            EditorGUILayout.PropertyField(clockPropertyProp);
            
            // 変更を適用
            serializedObject.ApplyModifiedProperties();
        }
    }
}