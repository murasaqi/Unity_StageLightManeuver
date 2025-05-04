using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace StageLightManeuver
{
    /// <summary>
    /// StageLightMasterClock Trackは時間関連の設定（BPM等）を一元管理するためのTrackです。
    /// 下位のStageLightTrackに影響を与えることができます。
    /// </summary>
    [TrackColor(0.5f, 0.8f, 0.8f)]
    [TrackClipType(typeof(StageLightMasterClockClip))]
    public class StageLightMasterClockTrack : TrackAsset
    {
        // 現在のClockの状態をシリアライズ可能な形で保持
        [SerializeField] private ClockProperty _currentClockProperty;
        
        // 読み取り専用プロパティ（外部からの参照用）
        public ClockProperty CurrentClockProperty
        {
            get
            {
                // nullチェックと初期化
                if (_currentClockProperty == null)
                {
                    _currentClockProperty = new ClockProperty();
                    // デフォルト値を設定
                    _currentClockProperty.bpm.value = 120f;
                    _currentClockProperty.bpmScale.value = 1f;
                }
                return _currentClockProperty;
            }
        }
        
        // 内部からの更新用メソッド
        internal void UpdateCurrentClockProperty(ClockProperty newProperty)
        {
            if (newProperty != null)
            {
                // nullチェックと初期化
                if (_currentClockProperty == null)
                {
                    _currentClockProperty = new ClockProperty();
                }
                
                // 値をコピー（参照ではなく）
                _currentClockProperty.bpm.value = newProperty.bpm.value;
                _currentClockProperty.bpmScale.value = newProperty.bpmScale.value;
                _currentClockProperty.offsetTime.value = newProperty.offsetTime.value;
                _currentClockProperty.staggerDelay.value = newProperty.staggerDelay.value;
                _currentClockProperty.loopType.value = newProperty.loopType.value;
                _currentClockProperty.arrayStaggerValue = newProperty.arrayStaggerValue;
                
                // clipPropertyがnullでない場合はそれもコピー
                if (newProperty.clipProperty != null)
                {
                    if (_currentClockProperty.clipProperty == null)
                    {
                        _currentClockProperty.clipProperty = new ClipProperty();
                    }
                    _currentClockProperty.clipProperty.clipStartTime = newProperty.clipProperty.clipStartTime;
                    _currentClockProperty.clipProperty.clipEndTime = newProperty.clipProperty.clipEndTime;
                }
                
                // デバッグ情報
                Debug.Log($"StageLightMasterClockTrack: Updated CurrentClockProperty - BPM = {_currentClockProperty.bpm.value}");
                
                // エディタでの更新を通知
#if UNITY_EDITOR
                UnityEditor.EditorUtility.SetDirty(this);
#endif
            }
        }
        
        // OnEnableでの初期化
        public void OnEnable()
        {
            // nullチェックと初期化
            if (_currentClockProperty == null)
            {
                _currentClockProperty = new ClockProperty();
                // デフォルト値を設定
                _currentClockProperty.bpm.value = 120f;
                _currentClockProperty.bpmScale.value = 1f;
            }
        }
        /// <summary>
        /// Mixerの作成
        /// </summary>
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            var mixer = ScriptPlayable<StageLightMasterClockMixer>.Create(graph, inputCount);
            var masterClockMixer = mixer.GetBehaviour();
            masterClockMixer.masterClockTrack = this;
            
            var timelineClips = GetClips().ToList();
            masterClockMixer.clips = timelineClips;
            
            foreach (var clip in timelineClips)
            {
                var masterClockClip = clip.asset as StageLightMasterClockClip;
                if (masterClockClip != null)
                {
                    masterClockClip.track = this;
                    masterClockClip.clipDisplayName = clip.displayName;
                }
            }
            
            return mixer;
        }
    }
}