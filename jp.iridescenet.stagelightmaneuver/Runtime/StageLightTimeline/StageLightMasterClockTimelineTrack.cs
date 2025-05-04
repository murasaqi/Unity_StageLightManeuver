using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace StageLightManeuver
{
    /// <summary>
    /// Clock Trackは時間関連の設定（BPM等）を一元管理するためのTrackです。
    /// 下位のStageLightTrackに影響を与えることができます。
    /// </summary>
    [TrackColor(0.5f, 0.8f, 0.8f)]
    [TrackClipType(typeof(StageLightMasterClockTimelineClip))]
    public class StageLightMasterClockTimelineTrack : TrackAsset
    {
        /// <summary>
        /// このTrackの下にある全てのTrackに影響を与えるかどうか
        /// </summary>
        [Tooltip("このTrackの下にある全てのTrackに影響を与えるかどうか")]
        public bool affectAllTracksBelow = true;
        
        /// <summary>
        /// Mixerの作成
        /// </summary>
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            var mixer = ScriptPlayable<StageLightMasterClockTimelineMixerBehaviour>.Create(graph, inputCount);
            var clockTimelineMixer = mixer.GetBehaviour();
            clockTimelineMixer.StageLightMasterClockTimelineTrack = this;
            
            var timelineClips = GetClips().ToList();
            clockTimelineMixer.clips = timelineClips;
            
            foreach (var clip in timelineClips)
            {
                var clockTimelineClip = clip.asset as StageLightMasterClockTimelineClip;
                if (clockTimelineClip != null)
                {
                    clockTimelineClip.track = this;
                    clockTimelineClip.clipDisplayName = clip.displayName;
                }
            }
            
            return mixer;
        }
    }
}