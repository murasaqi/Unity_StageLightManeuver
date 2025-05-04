using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace StageLightManeuver
{
    /// <summary>
    /// StageLightMasterBPM Trackは時間関連の設定（BPM等）を一元管理するためのTrackです。
    /// 下位のStageLightTrackに影響を与えることができます。
    /// </summary>
    [TrackColor(0.5f, 0.8f, 0.8f)]
    [TrackClipType(typeof(StageLightMasterBPMClip))]
    public class StageLightMasterBPMTrack : TrackAsset
    {
        // アクティブなクリップが存在するかどうか
        private bool _hasActiveClip = false;
        
        // Mixerへの参照
        private StageLightMasterBPMMixer _mixer;
        
        // アクティブなクリップとその重みのリスト
        private List<StageLightMasterBPMMixer.ClockClipInfo> _activeClips = new List<StageLightMasterBPMMixer.ClockClipInfo>();
        
        // アクティブなクリップが存在するかどうかを示すプロパティ
        public bool HasActiveClip => _hasActiveClip;
        
        // Mixerへのアクセスを提供するプロパティ
        public StageLightMasterBPMMixer Mixer => _mixer;
        
        // アクティブなクリップとその重みのリストを提供するプロパティ
        public IReadOnlyList<StageLightMasterBPMMixer.ClockClipInfo> ActiveClips => _activeClips;
        
        // アクティブなクリップの状態を設定するメソッド
        public void SetActiveClipState(bool hasActiveClip)
        {
            _hasActiveClip = hasActiveClip;
        }
        
        // アクティブなクリップを更新するメソッド
        internal void UpdateActiveClips(List<StageLightMasterBPMMixer.ClockClipInfo> activeClips)
        {
            _activeClips.Clear();
            _activeClips.AddRange(activeClips);
            _hasActiveClip = _activeClips.Count > 0;
        }
        
        // OnEnableでの初期化
        public void OnEnable()
        {
            // CurrentClockPropertyのgetterで初期化されるため、ここでは何もしない
        }
        /// <summary>
        /// Mixerの作成
        /// </summary>
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            var mixer = ScriptPlayable<StageLightMasterBPMMixer>.Create(graph, inputCount);
            var masterBPMMixer = mixer.GetBehaviour();
            masterBPMMixer.masterClockTrack = this;
            _mixer = masterBPMMixer;
            
            var timelineClips = GetClips().ToList();
            masterBPMMixer.clips = timelineClips;
            
            foreach (var clip in timelineClips)
            {
                var masterBPMClip = clip.asset as StageLightMasterBPMClip;
                if (masterBPMClip != null)
                {
                    masterBPMClip.track = this;
                    masterBPMClip.clipDisplayName = clip.displayName;
                }
            }
            
            return mixer;
        }
    }
}