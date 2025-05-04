using System;
using UnityEngine;
using UnityEngine.Playables;

namespace StageLightManeuver
{
    /// <summary>
    /// Clock Trackで使用するBehaviourクラス
    /// ClockPropertyを処理します
    /// </summary>
    [Serializable]
    public class StageLightMasterClockTimelineBehaviour : PlayableBehaviour
    {
        /// <summary>
        /// ClockPropertyの設定
        /// </summary>
        public ClockProperty clockProperty;
        
        /// <summary>
        /// クリップの表示名
        /// </summary>
        public string clipDisplayName;
        
        /// <summary>
        /// Playable作成時の処理
        /// </summary>
        public override void OnPlayableCreate(Playable playable)
        {
            if (clockProperty == null)
            {
                clockProperty = new ClockProperty();
            }
        }
        
        /// <summary>
        /// フレーム処理
        /// </summary>
        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            // ClockTimelineMixerBehaviourで処理するため、ここでは何もしない
        }
    }
}