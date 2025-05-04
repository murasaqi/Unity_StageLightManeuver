using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace StageLightManeuver
{
    /// <summary>
    /// PlayableDirectorの拡張メソッドを提供するクラス
    /// </summary>
    public static class PlayableDirectorExtensions
    {
        /// <summary>
        /// 指定されたトラックに影響を与えるClock Trackを見つけ、そのClockPropertyを取得します
        /// </summary>
        /// <param name="director">PlayableDirector</param>
        /// <param name="currentTrack">現在のトラック</param>
        /// <returns>見つかったClockProperty、見つからない場合はnull</returns>
        public static ClockProperty GetClockPropertyFromClockTrack(this PlayableDirector director, TrackAsset currentTrack)
        {
            if (director == null || currentTrack == null || director.playableAsset == null)
            {
                return null;
            }
            
            // TimelineAssetを取得
            var timelineAsset = director.playableAsset as TimelineAsset;
            if (timelineAsset == null)
            {
                return null;
            }
            
            // 全てのトラックを取得
            var allTracks = new List<TrackAsset>();
            foreach (var track in timelineAsset.GetRootTracks())
            {
                allTracks.Add(track);
                GetChildTracksRecursive(track, allTracks);
            }
            
            // 現在のトラックのインデックスを取得
            int currentTrackIndex = allTracks.IndexOf(currentTrack);
            if (currentTrackIndex < 0)
            {
                return null;
            }
            
            // 現在のトラックより上にあるClock Trackを検索
            for (int i = 0; i < currentTrackIndex; i++)
            {
                var track = allTracks[i];
                if (track is ClockTimelineTrack clockTrack && clockTrack.affectAllTracksBelow)
                {
                    // Clock Trackのミキサーを取得
                    var binding = director.GetGenericBinding(track);
                    var trackMixer = track.CreateTrackMixer(director.playableGraph, binding as GameObject, 1);
                    var scriptPlayable = (ScriptPlayable<ClockTimelineMixerBehaviour>)trackMixer;
                    var mixerBehaviour = scriptPlayable.GetBehaviour();
                    
                    // アクティブなClockPropertyを返す
                    return mixerBehaviour.ActiveClockProperty;
                }
            }
            
            return null;
        }
        
        /// <summary>
        /// 子トラックを再帰的に取得
        /// </summary>
        private static void GetChildTracksRecursive(TrackAsset track, List<TrackAsset> allTracks)
        {
            if (track == null)
            {
                return;
            }
            
            foreach (var childTrack in track.GetChildTracks())
            {
                allTracks.Add(childTrack);
                GetChildTracksRecursive(childTrack, allTracks);
            }
        }
    }
}