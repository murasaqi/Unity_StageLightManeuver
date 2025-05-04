using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace StageLightManeuver
{
    /// <summary>
    /// StageLightMasterBPM Trackで使用するClipクラス
    /// ClockPropertyを管理します
    /// </summary>
    [Serializable]
    public class StageLightMasterBPMClip : PlayableAsset, ITimelineClipAsset
    {
        /// <summary>
        /// BPMとBPM Scaleのみを持つ簡易設定クラス
        /// </summary>
        [Serializable]
        public class SimplifiedClockSettings
        {
            [Tooltip("Beats Per Minute")]
            public float bpm = 60f;
            
            [Tooltip("BPM Scale")]
            public float bpmScale = 1f;
        }
        
        /// <summary>
        /// 簡易化されたクロック設定（BPMとBPM Scaleのみ）
        /// </summary>
        public SimplifiedClockSettings clockSettings = new SimplifiedClockSettings();
        
        /// <summary>
        /// 内部使用のClockProperty
        /// </summary>
        [HideInInspector] public ClockProperty clockProperty = new ClockProperty();
        
        /// <summary>
        /// Behaviourインスタンス
        /// </summary>
        [HideInInspector] public StageLightMasterBPMBehaviour behaviour = new StageLightMasterBPMBehaviour();
        
        /// <summary>
        /// 所属するTrack
        /// </summary>
        [HideInInspector] public StageLightMasterBPMTrack track;
        
        /// <summary>
        /// クリップの表示名
        /// </summary>
        [HideInInspector] public string clipDisplayName;
        
        /// <summary>
        /// クリップの機能
        /// </summary>
        public ClipCaps clipCaps => ClipCaps.Blending;
        
        /// <summary>
        /// Playableの作成
        /// </summary>
        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<StageLightMasterBPMBehaviour>.Create(graph, behaviour);
            behaviour = playable.GetBehaviour();
            
            // 完全なClockPropertyを作成
            var fullClockProperty = new ClockProperty();
            
            // 簡易設定からBPMとBPM Scaleのみをコピー
            fullClockProperty.bpm.value = clockSettings.bpm;
            fullClockProperty.bpmScale.value = clockSettings.bpmScale;
            
            // BehaviourにClockPropertyを設定
            behaviour.clockProperty = fullClockProperty;
            behaviour.clipDisplayName = clipDisplayName;
            
            return playable;
        }
    }
}