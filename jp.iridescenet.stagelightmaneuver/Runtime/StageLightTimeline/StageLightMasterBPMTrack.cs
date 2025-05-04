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
        // 現在のClockの状態をシリアライズ可能な形で保持
        [SerializeField] private ClockProperty _currentClockProperty;
        
        // アクティブなクリップが存在するかどうか
        private bool _hasActiveClip = false;
        
        // Mixerへの参照
        private StageLightMasterBPMMixer _mixer;
        
        // アクティブなクリップが存在するかどうかを示すプロパティ
        public bool HasActiveClip => _hasActiveClip;
        
        // Mixerへのアクセスを提供するプロパティ
        public StageLightMasterBPMMixer Mixer => _mixer;
        
        // アクティブなクリップの状態を設定するメソッド
        public void SetActiveClipState(bool hasActiveClip)
        {
            _hasActiveClip = hasActiveClip;
        }
        
        // 読み取り専用プロパティ（外部からの参照用、後方互換性のため残す）
        public ClockProperty CurrentClockProperty
        {
            get
            {
                if (_currentClockProperty == null)
                {
                    _currentClockProperty = new ClockProperty();
                    // デフォルト値を設定
                    _currentClockProperty.bpm.value = 120f;
                    _currentClockProperty.bpmScale.value = 1f;
                }
                
                // アクティブなクリップがある場合は、最初のアクティブクリップの値を返す
                if (_mixer != null && _mixer.ActiveClockClips.Count > 0)
                {
                    var activeClip = _mixer.ActiveClockClips[0];
                    _currentClockProperty.bpm.value = activeClip.Property.bpm.value;
                    _currentClockProperty.bpmScale.value = activeClip.Property.bpmScale.value;
                    _currentClockProperty.offsetTime.value = activeClip.Property.offsetTime.value;
                }
                
                return _currentClockProperty;
            }
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