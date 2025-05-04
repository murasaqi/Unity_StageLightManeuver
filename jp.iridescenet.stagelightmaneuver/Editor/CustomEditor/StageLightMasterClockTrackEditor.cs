using UnityEditor;
using UnityEngine;

namespace StageLightManeuver
{
    /// <summary>
    /// StageLightMasterClockTrackのカスタムエディタ
    /// </summary>
    [CustomEditor(typeof(StageLightMasterClockTrack))]
    public class StageLightMasterClockTrackEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            EditorGUILayout.LabelField("StageLightMasterClock Track", EditorStyles.boldLabel);
            EditorGUILayout.Space();
            
            EditorGUILayout.HelpBox("このトラックは時間関連の設定（BPM等）を一元管理するためのトラックです。\n" +
                                   "下位のStageLightTrackに影響を与えることができます。", MessageType.Info);
            
            // 現在のClockPropertyの状態を表示（必要な情報のみ）
            var track = target as StageLightMasterClockTrack;
            if (track != null)
            {
                var currentProperty = track.CurrentClockProperty;
                if (currentProperty != null)
                {
                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("現在のClock設定", EditorStyles.boldLabel);
                    
                    // 必要な情報のみを表示
                    using (new EditorGUI.DisabledGroupScope(true)) // 読み取り専用
                    {
                        EditorGUILayout.FloatField("BPM", currentProperty.bpm.value);
                        EditorGUILayout.FloatField("BPMスケール", currentProperty.bpmScale.value);
                        EditorGUILayout.FloatField("オフセット時間", currentProperty.offsetTime.value);
                    }
                }
            }
            
            serializedObject.ApplyModifiedProperties();
        }
    }
}