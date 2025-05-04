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
        
        // Master BPM Trackの参照（Mixerは不要になる）
        private StageLightMasterBPMTrack _masterBPMTrack;
        private PlayableDirector _director;
        
        public override void OnGraphStart(Playable playable)
        {
            base.OnGraphStart(playable);
            
            // PlayableDirectorを取得
            _director = playable.GetGraph().GetResolver() as PlayableDirector;
            if (_director == null) return;
            
            // StageLightMasterBPMTrackを検索
            foreach (var output in _director.playableAsset.outputs)
            {
                if (output.sourceObject is StageLightMasterBPMTrack masterBPMTrack)
                {
                    _masterBPMTrack = masterBPMTrack;
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
            
            // 現在のアクティブなMaster BPM Clipsを取得
            List<ClockProperty> activeMasterBPMProperties = new List<ClockProperty>();
            if (_masterBPMTrack != null && _masterBPMTrack.HasActiveClip)
            {
                // Mixerから直接アクティブクリップを取得
                if (_masterBPMTrack.Mixer != null && _masterBPMTrack.Mixer.ActiveClockClips.Count > 0)
                {
                    foreach (var activeClip in _masterBPMTrack.Mixer.ActiveClockClips)
                    {
                        activeMasterBPMProperties.Add(activeClip.Property);
                    }
                }
                else
                {
                    // 後方互換性のため、Mixerが取得できない場合はCurrentClockPropertyを使用
                    activeMasterBPMProperties.Add(_masterBPMTrack.CurrentClockProperty);
                }
            }

            if (!firstFrameHappened)
            {
                trackBinding.Init();

                for (int i = 0; i < clips.Count; i++)
                {
                    var clip = clips[i];
                    var stageLightTimelineClip = clip.asset as StageLightTimelineClip;
                    if (stageLightTimelineClip == null) continue;
                    // Master BPM Propertiesがあり、かつアクティブなクリップが存在する場合のみマージ
                    UpdateProperty(clip, activeMasterBPMProperties);
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
                    // Master BPM Propertiesがあり、かつアクティブなクリップが存在する場合のみマージ
                    UpdateProperty(clip, activeMasterBPMProperties);
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
                        
                        // ClockPropertyの場合の処理
                        if (property is ClockProperty clockProperty)
                        {
                            // 新しいClockPropertyを作成
                            var mergedProperty = new ClockProperty(clockProperty);
                            
                            // クリップの開始・終了時間は保持
                            var clipStartTime = mergedProperty.clipProperty.clipStartTime;
                            var clipEndTime = mergedProperty.clipProperty.clipEndTime;
                            
                            // Ignore Master BPMフラグがtrueの場合は、クリップ自身のBPMとBPM Scaleを優先
                            if (!mergedProperty.ignoreMasterBPM && activeMasterBPMProperties.Count > 0)
                            {
                                // 複数のMaster BPM Propertiesがある場合、それらの値をブレンド
                                if (activeMasterBPMProperties.Count > 1)
                                {
                                    float bpm = 0f;
                                    float bpmScale = 0f;
                                    float totalWeight = 1.0f; // 簡略化のため、均等な重みを使用
                                    
                                    foreach (var masterProperty in activeMasterBPMProperties)
                                    {
                                        float weight = 1.0f / activeMasterBPMProperties.Count;
                                        bpm += masterProperty.bpm.value * weight;
                                        bpmScale += masterProperty.bpmScale.value * weight;
                                    }
                                    
                                    mergedProperty.bpm.value = bpm;
                                    mergedProperty.bpmScale.value = bpmScale;
                                }
                                else
                                {
                                    // 単一のMaster BPM Propertyの場合
                                    mergedProperty.bpm.value = activeMasterBPMProperties[0].bpm.value;
                                    mergedProperty.bpmScale.value = activeMasterBPMProperties[0].bpmScale.value;
                                }
                            }
                            // ignoreMasterBPM = trueの場合は、クリップ自身のBPMとBPM Scaleをそのまま使用
                            
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
        
        private void UpdateProperty(TimelineClip clip, List<ClockProperty> activeMasterBPMProperties)
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