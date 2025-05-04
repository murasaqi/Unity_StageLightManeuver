using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace StageLightManeuver
{
    /// <summary>
    /// Clock Trackで使用するMixerBehaviourクラス
    /// 複数のClockTimelineClipを処理し、現在のClockPropertyを管理します
    /// </summary>
    public class ClockTimelineMixerBehaviour : PlayableBehaviour
    {
        /// <summary>
        /// Timelineクリップのリスト
        /// </summary>
        public List<TimelineClip> clips;
        
        /// <summary>
        /// 所属するTrack
        /// </summary>
        public ClockTimelineTrack clockTimelineTrack;
        
        /// <summary>
        /// 現在アクティブなClockProperty
        /// </summary>
        private ClockProperty activeClockProperty;
        
        /// <summary>
        /// 現在アクティブなClockPropertyを取得
        /// </summary>
        public ClockProperty ActiveClockProperty => activeClockProperty;
        
        /// <summary>
        /// フレーム処理
        /// </summary>
        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            // 初期化
            if (activeClockProperty == null)
            {
                activeClockProperty = new ClockProperty();
            }
            
            // 入力の処理
            int inputCount = playable.GetInputCount();
            float highestWeight = 0f;
            ClockProperty weightedClockProperty = null;
            
            for (int i = 0; i < inputCount; i++)
            {
                float inputWeight = playable.GetInputWeight(i);
                
                if (inputWeight > 0)
                {
                    ScriptPlayable<ClockTimelineBehaviour> inputPlayable = (ScriptPlayable<ClockTimelineBehaviour>)playable.GetInput(i);
                    ClockTimelineBehaviour behaviour = inputPlayable.GetBehaviour();
                    
                    // 最も重みの高いClockPropertyを選択
                    if (inputWeight > highestWeight)
                    {
                        highestWeight = inputWeight;
                        weightedClockProperty = behaviour.clockProperty;
                    }
                }
            }
            
            // 最も重みの高いClockPropertyを適用
            if (weightedClockProperty != null)
            {
                activeClockProperty = weightedClockProperty;
            }
        }
    }
}