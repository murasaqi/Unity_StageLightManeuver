using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace StageLightManeuver
{
    /// <summary>
    /// Clock Trackで使用するClipクラス
    /// ClockPropertyを管理します
    /// </summary>
    [Serializable]
    public class ClockTimelineClip : PlayableAsset, ITimelineClipAsset
    {
        /// <summary>
        /// ClockPropertyの設定
        /// </summary>
        public ClockProperty clockProperty = new ClockProperty();
        
        /// <summary>
        /// Behaviourインスタンス
        /// </summary>
        [HideInInspector] public ClockTimelineBehaviour behaviour = new ClockTimelineBehaviour();
        
        /// <summary>
        /// 所属するTrack
        /// </summary>
        [HideInInspector] public ClockTimelineTrack track;
        
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
            var playable = ScriptPlayable<ClockTimelineBehaviour>.Create(graph, behaviour);
            behaviour = playable.GetBehaviour();
            behaviour.clockProperty = new ClockProperty(clockProperty);
            behaviour.clipDisplayName = clipDisplayName;
            
            return playable;
        }
    }
}