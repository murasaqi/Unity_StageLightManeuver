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

        private static readonly HashSet<string> overwriteExceptionPropNames = new()
        {
            "Clock",
            "StageLight Order"
        };

        private List<StageLightQueueData> composedQueueDatas;
        private HashSet<string> alreadyAddedPropNames = new();
        private List<StageLightQueueData> QueueDatas => composedQueueDatas;

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {

            trackBinding = playerData as StageLightFixtureBase;
            var hasAnyClipPlaying = false;
            var time = playable.GetTime();

            if (!trackBinding)
                return;

            int inputCount = playable.GetInputCount();
            composedQueueDatas ??= new List<StageLightQueueData>();
            composedQueueDatas.Clear();

            for (int i = 0; i < inputCount; i++)
            {
                var input = playable.GetInput(inputCount - 1 - i);
                var trackMixer = ((ScriptPlayable<StageLightTimelineMixerBehaviour>)input).GetBehaviour();
                var queueDatas = trackMixer.QueueDatas;
                alreadyAddedPropNames.Clear();
                foreach (var queueData in composedQueueDatas)
                {
                    foreach (var propName in queueData.stageLightProperties.Where(x => x.propertyOverride)
                                .Select(x => x.propertyName))
                    {
                        if (!overwriteExceptionPropNames.Contains(propName))
                            alreadyAddedPropNames.Add(propName);
                    }
                }

                foreach (var q in queueDatas)
                {
                    // 重複しているプロパティを削除
                    var j = 0;
                    while (q.stageLightProperties.Count > j)
                    {
                        var prop = q.stageLightProperties[j];
                        if (alreadyAddedPropNames.Contains(prop.propertyName))
                        {
                            q.stageLightProperties.RemoveAt(j);
                        }
                        else
                        {
                            j++;
                        }
                    }
                }

                composedQueueDatas.AddRange(queueDatas);
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
