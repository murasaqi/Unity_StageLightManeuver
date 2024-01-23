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


            var hasAnyClipPlaying = false;
            var time = playable.GetTime();
            for (int i = 0; i < clips.Count; i++)
            {
                var clip = clips[i];
                var stageLightTimelineClip = clip.asset as StageLightTimelineClip;
                if (stageLightTimelineClip == null) continue;
                float inputWeight = playable.GetInputWeight(i);
                var timeProperty = stageLightTimelineClip.StageLightQueueData.TryGetActiveProperty<ClockProperty>();
                if (timeProperty != null)
                {
                    timeProperty.clipProperty.clipStartTime = (float)clip.start;
                    timeProperty.clipProperty.clipEndTime = (float)clip.end;
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
                    trackBinding.AddQue(stageLightTimelineClip.StageLightQueueData);
                    hasAnyClipPlaying = true;
                }
            }

            if (stageLightTimelineTrack)
            {
                Debug.Log(stageLightTimelineTrack.useIntensityMultiplier + ": " + stageLightTimelineTrack.intensityMultiplier);
                if (!hasAnyClipPlaying)
                {
                    if (stageLightTimelineTrack.updateOnOutOfClip) trackBinding.EvaluateQue((float)time);

                    // LightIntensityはTrack側intensityMultiplierを考慮する
                    if (stageLightTimelineTrack.useIntensityMultiplier) 
                    {
                        foreach (var stageLightFixture in trackBinding.stageLightFixtures)
                        {
                            foreach (var stageLightChannel in stageLightFixture.StageLightChannels)
                            {
                                if (stageLightChannel is LightChannel)
                                {
                                    var lightChannel = stageLightChannel as LightChannel;
                                    if (lightChannel != null)
                                    {
                                        lightChannel.lightIntensity *= Mathf.Max(0f, stageLightTimelineTrack.intensityMultiplier);   
                                    }
                                }
                            }
                        }
                    }

                    trackBinding.UpdateChannel();
                }
                else
                {
                    trackBinding.EvaluateQue((float)time);

                    // LightIntensityはTrack側intensityMultiplierを考慮する
                    if (stageLightTimelineTrack.useIntensityMultiplier) 
                    {
                        foreach (var stageLightFixture in trackBinding.stageLightFixtures)
                        {
                            foreach (var stageLightChannel in stageLightFixture.StageLightChannels)
                            {
                                if (stageLightChannel is LightChannel)
                                {
                                    var lightChannel = stageLightChannel as LightChannel;
                                    if (lightChannel != null)
                                    {
                                        lightChannel.lightIntensity *= Mathf.Max(0f, stageLightTimelineTrack.intensityMultiplier);   
                                    }
                                }
                            }
                        }
                    }

                    trackBinding.UpdateChannel();
                }
            }

        }

       
        

        
        
    }
}