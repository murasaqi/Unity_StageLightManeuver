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
        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            trackBinding = playerData as StageLightFixtureBase;

            if (!trackBinding)
                return;

            if (firstFrameHappened)
            {
                trackBinding.Init();
                firstFrameHappened = true;
            }
            
            queueDatas.Clear();

            var hasAnyClipPlaying = false;
            for (int i = 0; i < clips.Count; i++)
            {
                var clip = clips[i];
                var stageLightTimelineClip = clip.asset as StageLightTimelineClip;
                if (stageLightTimelineClip == null) continue;
                float inputWeight = playable.GetInputWeight(i);
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
                
                if (inputWeight > 0)
                {
                    stageLightTimelineClip.StageLightQueueData.weight = inputWeight;
                    queueDatas.Add(stageLightTimelineClip.StageLightQueueData);
                    hasAnyClipPlaying = true;
                }
            }

        }

       
        

        
        
    }
}