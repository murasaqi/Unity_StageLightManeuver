using UnityEditor;
using UnityEngine;

namespace StageLightManeuver
{
    /// <summary>
    /// StageLightMasterBPMTrackのカスタムエディタ
    /// </summary>
    [CustomEditor(typeof(StageLightMasterBPMTrack))]
    public class StageLightMasterBPMTrackEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            EditorGUILayout.LabelField("StageLightMasterBPM Track", EditorStyles.boldLabel);
            EditorGUILayout.Space();
            
            EditorGUILayout.HelpBox("このトラックは時間関連の設定（BPM等）を一元管理するためのトラックです。\n" +
                                   "下位のStageLightTrackに影響を与えることができます。", MessageType.Info);
            
            // 現在のClockPropertyの状態を表示
            var track = target as StageLightMasterBPMTrack;
            if (track != null)
            {
                EditorGUILayout.Space();
                
                // アクティブなクリップの情報を表示
                if (track.HasActiveClip && track.Mixer != null && track.Mixer.ActiveClockClips.Count > 0)
                {
                    EditorGUILayout.LabelField("アクティブなクリップ", EditorStyles.boldLabel);
                    
                    for (int i = 0; i < track.Mixer.ActiveClockClips.Count; i++)
                    {
                        var activeClip = track.Mixer.ActiveClockClips[i];
                        EditorGUILayout.LabelField($"クリップ {i + 1}", EditorStyles.boldLabel);
                        
                        using (new EditorGUI.DisabledGroupScope(true)) // 読み取り専用
                        {
                            EditorGUILayout.FloatField("BPM", activeClip.Property.bpm.value);
                            EditorGUILayout.FloatField("BPMスケール", activeClip.Property.bpmScale.value);
                            EditorGUILayout.FloatField("オフセット時間", activeClip.Property.offsetTime.value);
                            EditorGUILayout.FloatField("重み", activeClip.Weight);
                            EditorGUILayout.TextField("開始時間", activeClip.StartTime.ToString("F2"));
                            EditorGUILayout.TextField("終了時間", activeClip.EndTime.ToString("F2"));
                        }
                        
                        EditorGUILayout.Space();
                    }
                }
                else
                {
                    // アクティブなクリップがない場合は、デフォルト値を表示
                    EditorGUILayout.LabelField("現在のClock設定", EditorStyles.boldLabel);
                    EditorGUILayout.HelpBox("現在アクティブなクリップはありません。デフォルト値を表示しています。", MessageType.Info);
                    
                    // デフォルト値を表示
                    using (new EditorGUI.DisabledGroupScope(true)) // 読み取り専用
                    {
                        EditorGUILayout.FloatField("BPM", 120f);
                        EditorGUILayout.FloatField("BPMスケール", 1f);
                        EditorGUILayout.FloatField("オフセット時間", 0f);
                    }
                }
            }
            
            serializedObject.ApplyModifiedProperties();
        }
    }
}