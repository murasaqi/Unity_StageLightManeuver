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

        public List<TimelineClip> clips;

        public StageLightTimelineTrack stageLightTimelineTrack;
        private bool firstFrameHappened = false;

        public StageLightFixtureBase trackBinding;
        private List<StageLightQueueData> queueDatas = new();
        public List<StageLightQueueData> QueueDatas => queueDatas;
        
        // MasterClockTrackの参照（Mixerは不要になる）
        private StageLightMasterClockTrack _masterClockTrack;
        private PlayableDirector _director;
        
        public override void OnGraphStart(Playable playable)
        {
            base.OnGraphStart(playable);
            
            // PlayableDirectorを取得
            _director = playable.GetGraph().GetResolver() as PlayableDirector;
            if (_director == null) return;
            
            // StageLightMasterClockTrackを検索
            foreach (var output in _director.playableAsset.outputs)
            {
                if (output.sourceObject is StageLightMasterClockTrack masterClockTrack)
                {
                    _masterClockTrack = masterClockTrack;
                    break;
                }
            }
        }
        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            trackBinding = playerData as StageLightFixtureBase;

            if (!trackBinding)
                return;
                
            // 現在の時間を取得
            double currentTime = _director != null ? _director.time : 0;
            
            // 現在のMasterClockPropertyを取得（簡略化）
            ClockProperty masterClockProperty = null;
            if (_masterClockTrack != null)
            {
                masterClockProperty = _masterClockTrack.CurrentClockProperty;
            }

            if (!firstFrameHappened)
            {
                trackBinding.Init();

                for (int i = 0; i < clips.Count; i++)
                {
                    var clip = clips[i];
                    var stageLightTimelineClip = clip.asset as StageLightTimelineClip;
                    if (stageLightTimelineClip == null) continue;
                    // MasterClockPropertyがあり、かつアクティブなクリップが存在する場合のみマージ
                    UpdateProperty(clip, _masterClockTrack != null && _masterClockTrack.HasActiveClip ? masterClockProperty : null);
                }
                firstFrameHappened = true;
            }
            
            queueDatas.Clear();

            for (int i = 0; i < clips.Count; i++)
            {
                var clip = clips[i];
                var stageLightTimelineClip = clip.asset as StageLightTimelineClip;
                if (stageLightTimelineClip == null) continue;
                float inputWeight = playable.GetInputWeight(i);
                
#if UNITY_EDITOR
                if (!Application.isPlaying)
                {
                    // MasterClockPropertyがあり、かつアクティブなクリップが存在する場合のみマージ
                    UpdateProperty(clip, _masterClockTrack != null && _masterClockTrack.HasActiveClip ? masterClockProperty : null);
                }
#endif
                
                if (inputWeight > 0)
                {
                    // StageLightQueueDataのコピーを作成
                    var queueData = new StageLightQueueData();
                    queueData.weight = inputWeight;
                    
                    // プロパティをコピー
                    foreach (var property in stageLightTimelineClip.StageLightQueueData.stageLightProperties)
                    {
                        if (property == null) continue;
                        
                        // ClockPropertyの場合、MasterClockPropertyがあり、かつアクティブなクリップが存在する場合のみマージ
                        if (property is ClockProperty clockProperty && masterClockProperty != null && _masterClockTrack.HasActiveClip)
                        {
                            // 新しいClockPropertyを作成してマージ
                            var mergedProperty = new ClockProperty(clockProperty);
                            
                            // クリップの開始・終了時間は保持
                            var clipStartTime = mergedProperty.clipProperty.clipStartTime;
                            var clipEndTime = mergedProperty.clipProperty.clipEndTime;
                            
                            // Ignore MasterClockフラグがtrueの場合は、クリップ自身のBPMとBPM Scaleを優先
                            // falseの場合はMasterClockのBPMとBPM Scaleを使用
                            if (!mergedProperty.ignoreMasterClock)
                            {
                                // MasterClockPropertyからBPMとBPM Scaleを使用
                                mergedProperty.bpm.value = masterClockProperty.bpm.value;
                                mergedProperty.bpmScale.value = masterClockProperty.bpmScale.value;
                            }
                            // ignoreMasterClock = trueの場合は、クリップ自身のBPMとBPM Scaleをそのまま使用
                            
                            // クリップの開始・終了時間を復元
                            mergedProperty.clipProperty.clipStartTime = clipStartTime;
                            mergedProperty.clipProperty.clipEndTime = clipEndTime;
                            
                            
                            queueData.stageLightProperties.Add(mergedProperty);
                        }
                        else
                        {
                            // その他のプロパティはそのままコピー
                            queueData.stageLightProperties.Add(property);
                        }
                    }
                    
                    queueDatas.Add(queueData);
                }
            }
            
            // キューデータをトラックバインディングに追加
            foreach (var queueData in queueDatas)
            {
                trackBinding.AddQue(queueData);
            }
        }
        
        private void UpdateProperty(TimelineClip clip, ClockProperty masterClockProperty = null)
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
    }
}