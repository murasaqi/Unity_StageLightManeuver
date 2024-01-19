using UnityEngine;

namespace StageLightManeuver
{
    public static class SLSUtility
    {
        // public static float GetNormalizedTime(float time,float bpm, float bpmScale,float bpmOffset,int index,LoopType loopType)
        // {
        //     
        //     var scaledBpm = bpm * bpmScale;
        //     var duration = 60 / scaledBpm;
        //     var offset = duration* bpmOffset * index;
        //     var offsetTime = time + offset;
        //     var result = 0f;
        //     var t = (float)offsetTime % duration;
        //     var normalisedTime = t / duration;
        //     
        //     if (loopType == LoopType.Loop)
        //     {
        //         result = normalisedTime;     
        //     }else if (loopType == LoopType.PingPong)
        //     {
        //         result = Mathf.PingPong(offsetTime / duration, 1f);
        //     }
        //     else if(loopType == LoopType.Fixed)
        //     {
        //         result = Mathf.InverseLerp(clipProperty.clipStartTime, clipProperty.clipEndTime, normalisedTime);
        //     }
        //    
        //     return result;
        // }
    }
}