using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace StageLightManeuver
{
    /// <summary>
    /// StageLightMasterBPM Trackで使用するMixerBehaviourクラス
    /// 複数のStageLightMasterBPMClipを処理し、現在のClockPropertyを管理します
    /// </summary>
    public class StageLightMasterBPMMixer : PlayableBehaviour
    {
        /// <summary>
        /// Timelineクリップのリスト
        /// </summary>
        public List<TimelineClip> clips;
        
        /// <summary>
        /// 所属するTrack
        /// </summary>
        public StageLightMasterBPMTrack masterClockTrack;
        
        // アクティブなMasterBPMClipとその情報を追跡するためのクラス
        public class ClockClipInfo
        {
            public ClockProperty Property { get; set; }
            public float Weight { get; set; }
            public double StartTime { get; set; }
            public double EndTime { get; set; }
        }
        
        // 現在アクティブなMasterBPMClipとその情報を追跡
        private List<ClockClipInfo> _activeClockClips = new List<ClockClipInfo>();
        
        // 外部からアクティブなクリップにアクセスするためのプロパティ
        public IReadOnlyList<ClockClipInfo> ActiveClockClips => _activeClockClips;
        
        /// <summary>
        /// フレーム処理
        /// </summary>
        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            // 各クリップの重みを更新
            _activeClockClips.Clear();
            
            int inputCount = playable.GetInputCount();
            for (int i = 0; i < inputCount; i++)
            {
                float inputWeight = playable.GetInputWeight(i);
                
                if (i < clips.Count && inputWeight > 0)
                {
                    var clip = clips[i];
                    var masterBPMClip = clip.asset as StageLightMasterBPMClip;
                    if (masterBPMClip != null)
                    {
                        // クリップの情報を登録
                        _activeClockClips.Add(new ClockClipInfo
                        {
                            Property = masterBPMClip.behaviour.clockProperty,
                            Weight = inputWeight,
                            StartTime = clip.start,
                            EndTime = clip.end
                        });
                    }
                }
            }
            
            // 現在の時間を取得
            double currentTime = 0;
            
            // PlayableDirectorを取得する方法を改善
            PlayableDirector director = null;
            
            // まずplayerDataからPlayableDirectorを取得
            director = playerData as PlayableDirector;
            
            // playerDataがPlayableDirectorでない場合、PlayableGraphから取得
            if (director == null)
            {
                var resolver = playable.GetGraph().GetResolver();
                director = resolver as PlayableDirector;
            }
            
            // PlayableDirectorが見つかった場合、現在の時間を取得
            if (director != null)
            {
                currentTime = director.time;
            }
            else
            {
                // PlayableDirectorが見つからない場合、Playableから時間を取得
                currentTime = playable.GetTime();
            }
            
            // アクティブなクリップを更新
            UpdateActiveClipsAtTime(currentTime);
            
            // TrackのHasActiveClip状態を更新
            if (masterClockTrack != null)
            {
                masterClockTrack.SetActiveClipState(_activeClockClips.Count > 0);
            }
        }
        
        /// <summary>
        /// 指定された時間にアクティブなクリップを更新するメソッド
        /// </summary>
        private void UpdateActiveClipsAtTime(double time)
        {
            // 現在のアクティブクリップをクリア
            _activeClockClips.Clear();
            
            // すべてのクリップをチェックして、現在の時間にアクティブなものを追加
            if (clips != null)
            {
                foreach (var clip in clips)
                {
                    if (time >= clip.start && time <= clip.end)
                    {
                        var masterBPMClip = clip.asset as StageLightMasterBPMClip;
                        if (masterBPMClip != null)
                        {
                            // クリップの重みを計算（エッジでのブレンドを考慮）
                            float weight = 1.0f;
                            
                            // クリップの情報を登録
                            _activeClockClips.Add(new ClockClipInfo
                            {
                                Property = masterBPMClip.behaviour.clockProperty,
                                Weight = weight,
                                StartTime = clip.start,
                                EndTime = clip.end
                            });
                        }
                    }
                }
            }
        }
    }
}