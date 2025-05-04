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
            
            // MasterBPMTrackからアクティブクリップを取得
            List<StageLightMasterBPMMixer.ClockClipInfo> activeMasterBPMClips = new List<StageLightMasterBPMMixer.ClockClipInfo>();
            if (_masterBPMTrack != null && _masterBPMTrack.HasActiveClip)
            {
                // Trackから直接アクティブクリップを取得
                activeMasterBPMClips.AddRange(_masterBPMTrack.ActiveClips);
            }

            if (!firstFrameHappened)
            {
                trackBinding.Init();

                for (int i = 0; i < clips.Count; i++)
                {
                    var clip = clips[i];
                    var stageLightTimelineClip = clip.asset as StageLightTimelineClip;
                    if (stageLightTimelineClip == null) continue;
                    // Master BPM Clipsがあり、かつアクティブなクリップが存在する場合のみマージ
                    UpdateProperty(clip, activeMasterBPMClips);
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
                    // Master BPM Clipsがあり、かつアクティブなクリップが存在する場合のみマージ
                    UpdateProperty(clip, activeMasterBPMClips);
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
                            // Ignore Master BPMフラグがtrueの場合は、クリップ自身のBPMとBPM Scaleを優先
                            if (!clockProperty.ignoreMasterBPM && activeMasterBPMClips.Count > 0)
                            {
                                // 各MasterBPMClipに対して別々のQueueDataを作成
                                foreach (var bpmClip in activeMasterBPMClips)
                                {
                                    // 新しいQueueDataを作成
                                    var bpmQueueData = new StageLightQueueData();
                                    
                                    // QueueDataの重みはStageLightTimelineClipの重みをそのまま使用
                                    bpmQueueData.weight = queueData.weight;
                                    
                                    // 新しいClockPropertyを作成
                                    var bpmProperty = new ClockProperty(clockProperty);
                                    
                                    // ClockPropertyの重みをMasterBPMTrackのクリップの重みに設定
                                    bpmProperty.SetWeight(bpmClip.Weight);
                                    
                                    // MasterBPMClipのプロパティを適用
                                    bpmProperty.bpm.value = bpmClip.Property.bpm.value;
                                    bpmProperty.bpmScale.value = bpmClip.Property.bpmScale.value;
                                    
                                    // クリップの開始・終了時間を設定
                                    bpmProperty.clipProperty.clipStartTime = (float)bpmClip.StartTime;
                                    bpmProperty.clipProperty.clipEndTime = (float)bpmClip.EndTime;
                                    
                                    // プロパティをQueueDataに追加
                                    bpmQueueData.stageLightProperties.Add(bpmProperty);
                                    
                                    // その他のプロパティもコピー（重みはリセット）
                                    foreach (var otherProperty in stageLightTimelineClip.StageLightQueueData.stageLightProperties)
                                    {
                                        if (otherProperty != null && !(otherProperty is ClockProperty))
                                        {
                                            // 他のプロパティをコピー
                                            var copiedProperty = Activator.CreateInstance(otherProperty.GetType(), otherProperty) as SlmProperty;
                                            
                                            // 重みをリセット（QueueDataのデフォルト重みを使用）
                                            copiedProperty.ResetWeight();
                                            
                                            bpmQueueData.stageLightProperties.Add(copiedProperty);
                                        }
                                    }
                                    
                                    // QueueDataをリストに追加
                                    queueDatas.Add(bpmQueueData);
                                }
                                
                                // 元のQueueDataは追加しない
                                continue;
                            }
                            else
                            {
                                // ignoreMasterBPM = trueの場合は、クリップ自身のBPMとBPM Scaleをそのまま使用
                                queueData.stageLightProperties.Add(property);
                            }
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
        
        private void UpdateProperty(TimelineClip clip, List<StageLightMasterBPMMixer.ClockClipInfo> activeMasterBPMClips)
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