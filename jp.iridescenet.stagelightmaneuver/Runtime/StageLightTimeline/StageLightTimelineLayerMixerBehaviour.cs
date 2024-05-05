using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace StageLightManeuver
{
    public class StageLightTimelineLayerMixerBehaviour : PlayableBehaviour
    {
        
        public List<TimelineClip> clips;

        public StageLightTimelineTrack stageLightTimelineTrack;

        public StageLightFixtureBase trackBinding;

        private List<string> overwriteExceptionPropNames = new List<string>() {"Clock","StageLight Order"};
        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            
            trackBinding = playerData as StageLightFixtureBase;
            var hasAnyClipPlaying = false;
            var time = playable.GetTime();

            if (!trackBinding)
                return;

            int inputCount = playable.GetInputCount();
            var composedQueueDatas = new List<StageLightQueueData>();
            for (int i = 0; i < inputCount; i++)
            {
                var input = playable.GetInput(inputCount - 1 - i);
                var trackMixer = ((ScriptPlayable<StageLightTimelineMixerBehaviour>) input).GetBehaviour();
                var queueDatas = trackMixer.QueueDatas;
                var alreadyAddedPropNames = new List<string>();
                foreach (var queueData in composedQueueDatas)
                {
                    foreach (var propName in queueData.stageLightProperties.Where(x => x.propertyOverride).Select(x => x.propertyName))
                    {
                        if(!overwriteExceptionPropNames.Contains(propName))
                            alreadyAddedPropNames.Add(propName);
                    }
                }

                var str = "";
                foreach (var propName in alreadyAddedPropNames)
                {
                    str += propName + ",";
                }
                // Debug.Log(str);

                foreach (var queueData in queueDatas)
                {
                    var tmpQueueData = new StageLightQueueData();
                    tmpQueueData.stageLightOrder = queueData.stageLightOrder;
                    tmpQueueData.weight = queueData.weight;
                    foreach (var prop in queueData.stageLightProperties)
                    {
                        if (!alreadyAddedPropNames.Contains(prop.propertyName))
                        {
                            tmpQueueData.stageLightProperties.Add(prop);
                        }
                    }
                    composedQueueDatas.Add(tmpQueueData);
                }
            }

            for (int i = 0; i < composedQueueDatas.Count; i++)
            {
                trackBinding.AddQue(composedQueueDatas[composedQueueDatas.Count - 1 - i]);
                hasAnyClipPlaying = true;
            }
            
            if (stageLightTimelineTrack)
            {
                if (!hasAnyClipPlaying)
                {
                    if (stageLightTimelineTrack.updateOnOutOfClip) trackBinding.EvaluateQue((float)time);
                    trackBinding.UpdateChannel();
                }
                else
                {
                    trackBinding.EvaluateQue((float)time);
                    trackBinding.UpdateChannel();
                }
            }
        }
    }
}