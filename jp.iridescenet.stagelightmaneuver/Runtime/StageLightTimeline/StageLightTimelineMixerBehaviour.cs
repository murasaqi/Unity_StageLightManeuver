using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace StageLightManeuver
{
    public class StageLightTimelineMixerBehaviour : PlayableBehaviour
    {
        /// <summary>
        /// Timelineクリップのリスト
        /// </summary>
        public List<TimelineClip> clips;

        /// <summary>
        /// 所属するTrack
        /// </summary>
        public StageLightTimelineTrack stageLightTimelineTrack;
        
        private bool firstFrameHappened = false;

        /// <summary>
        /// トラックのバインディング
        /// </summary>
        public StageLightFixtureBase trackBinding;
        
        private List<StageLightQueueData> queueDatas = new();
        
        /// <summary>
        /// キューデータのリスト
        /// </summary>
        public List<StageLightQueueData> QueueDatas => queueDatas;
        
        /// <summary>
        /// Clock Trackの設定を受け入れるかどうか
        /// </summary>
        public bool acceptClockTrackSettings = true;
        /// <summary>
        /// フレーム処理
        /// </summary>
        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            trackBinding = playerData as StageLightFixtureBase;

            if (!trackBinding)
                return;

            if (!firstFrameHappened)
            {
                trackBinding.Init();

                for (int i = 0; i < clips.Count; i++)
                {
                    var clip = clips[i];
                    var stageLightTimelineClip = clip.asset as StageLightTimelineClip;
                    if (stageLightTimelineClip == null) continue;
                    UpdateProperty(clip);
                }
                firstFrameHappened = true;
            }
            
            queueDatas.Clear();

            var hasAnyClipPlaying = false;
            
            // Clock Trackからの設定を取得
            var playableDirector = trackBinding.GetComponentInParent<PlayableDirector>();
            ClockProperty clockFromTrack = null;
            
            if (playableDirector != null && acceptClockTrackSettings)
            {
                clockFromTrack = playableDirector.GetClockPropertyFromClockTrack(stageLightTimelineTrack);
            }
            
            for (int i = 0; i < clips.Count; i++)
            {
                var clip = clips[i];
                var stageLightTimelineClip = clip.asset as StageLightTimelineClip;
                if (stageLightTimelineClip == null) continue;
                float inputWeight = playable.GetInputWeight(i);
                
#if UNITY_EDITOR
                if (!Application.isPlaying)
                {
                    UpdateProperty(clip);
                }
#endif
                
                if (inputWeight > 0)
                {
                    // Clock Trackの設定を適用（クリップが受け入れる場合のみ）
                    if (clockFromTrack != null && stageLightTimelineClip.acceptClockTrackSettings)
                    {
                        ApplyClockSettings(stageLightTimelineClip, clockFromTrack);
                    }
                    
                    stageLightTimelineClip.StageLightQueueData.weight = inputWeight;
                    queueDatas.Add(stageLightTimelineClip.StageLightQueueData);
                    hasAnyClipPlaying = true;
                }
            }
        }
        
        /// <summary>
        /// プロパティの更新
        /// </summary>
        private void UpdateProperty(TimelineClip clip)
        {
            var stageLightTimelineClip = clip.asset as StageLightTimelineClip;
            if (stageLightTimelineClip == null) return;
            var clockProperty = stageLightTimelineClip.StageLightQueueData.TryGetActiveProperty<ClockProperty>();
            if (clockProperty != null)
            {
                clockProperty.clipProperty.clipStartTime = (float)clip.start;
                clockProperty.clipProperty.clipEndTime = (float)clip.end;
            }

            foreach (var stageLightProperty in stageLightTimelineClip.StageLightQueueData.stageLightProperties)
            {
                if(stageLightProperty == null) continue;
                stageLightProperty.InitStageLightFixture(trackBinding);
                if (stageLightProperty.propertyType == StageLightPropertyType.Array )
                {
                    var additionalArrayProperty = stageLightProperty as IArrayProperty;
                    additionalArrayProperty?.ResyncArraySize(trackBinding.stageLightFixtures);
                }
            }
        }
        
        /// <summary>
        /// Clock Trackの設定を適用
        /// </summary>
        private void ApplyClockSettings(StageLightTimelineClip clip, ClockProperty clockFromTrack)
        {
            if (clip == null || clockFromTrack == null) return;
            
            // クリップのClockPropertyを取得
            var clipClockProperty = clip.StageLightQueueData.TryGetActiveProperty<ClockProperty>();
            if (clipClockProperty == null) return;
            
            // プロパティ単位での継承制御
            if (clockFromTrack.bpm.propertyOverride && clipClockProperty.bpm.propertyOverride)
            {
                clipClockProperty.bpm.value = clockFromTrack.bpm.value;
            }
            
            if (clockFromTrack.bpmScale.propertyOverride && clipClockProperty.bpmScale.propertyOverride)
            {
                clipClockProperty.bpmScale.value = clockFromTrack.bpmScale.value;
            }
            
            if (clockFromTrack.loopType.propertyOverride && clipClockProperty.loopType.propertyOverride)
            {
                clipClockProperty.loopType.value = clockFromTrack.loopType.value;
            }
            
            if (clockFromTrack.offsetTime.propertyOverride && clipClockProperty.offsetTime.propertyOverride)
            {
                clipClockProperty.offsetTime.value = clockFromTrack.offsetTime.value;
            }
        }
    }
}