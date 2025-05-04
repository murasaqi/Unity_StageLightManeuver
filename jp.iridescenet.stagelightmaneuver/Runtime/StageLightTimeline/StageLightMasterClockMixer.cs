using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace StageLightManeuver
{
    /// <summary>
    /// StageLightMasterClock Trackで使用するMixerBehaviourクラス
    /// 複数のStageLightMasterClockClipを処理し、現在のClockPropertyを管理します
    /// </summary>
    public class StageLightMasterClockMixer : PlayableBehaviour
    {
        /// <summary>
        /// Timelineクリップのリスト
        /// </summary>
        public List<TimelineClip> clips;
        
        /// <summary>
        /// 所属するTrack
        /// </summary>
        public StageLightMasterClockTrack masterClockTrack;
        
        // アクティブなMasterClockClipとその情報を追跡するためのクラス
        private class ClockClipInfo
        {
            public ClockProperty Property { get; set; }
            public float Weight { get; set; }
            public double StartTime { get; set; }
            public double EndTime { get; set; }
        }
        
        // 現在アクティブなMasterClockClipとその情報を追跡
        private List<ClockClipInfo> _activeClockClips = new List<ClockClipInfo>();
        
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
                    var masterClockClip = clip.asset as StageLightMasterClockClip;
                    if (masterClockClip != null)
                    {
                        // クリップの情報を登録
                        _activeClockClips.Add(new ClockClipInfo
                        {
                            Property = masterClockClip.behaviour.clockProperty,
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
            
            // デバッグ情報
            Debug.Log($"StageLightMasterClockMixer: CurrentTime = {currentTime}");
            
            // 現在のClockPropertyを計算
            var currentClockProperty = GetActiveClockPropertyAtTime(currentTime);
            
            // TrackのCurrentClockPropertyを更新
            if (masterClockTrack != null)
            {
                if (currentClockProperty != null)
                {
                    masterClockTrack.UpdateCurrentClockProperty(currentClockProperty);
                    
                    // デバッグ情報
                    Debug.Log($"StageLightMasterClockMixer: Updated CurrentClockProperty - BPM = {currentClockProperty.bpm.value}");
                }
                else
                {
                    // アクティブなクリップがない場合はデフォルト値を設定
                    var defaultProperty = new ClockProperty();
                    defaultProperty.bpm.value = 120f; // デフォルトBPM
                    defaultProperty.bpmScale.value = 1f;
                    masterClockTrack.UpdateCurrentClockProperty(defaultProperty);
                    
                    Debug.Log("StageLightMasterClockMixer: No active clips, using default property");
                }
            }
        }
        
        /// <summary>
        /// 指定された時間にアクティブなClockPropertyを取得するメソッド
        /// 複数のClockPropertyがアクティブな場合は、それらの値をブレンドします
        /// </summary>
        public ClockProperty GetActiveClockPropertyAtTime(double time)
        {
            // 現在の時間に有効なクリップを取得
            var activeClips = _activeClockClips.Where(clip =>
                time >= clip.StartTime && time <= clip.EndTime).ToList();
            
            if (activeClips.Count == 0)
            {
                return null;
            }
            
            if (activeClips.Count == 1)
            {
                return activeClips[0].Property;
            }
            
            // 複数のクリップがアクティブな場合、線形補間でブレンド
            var blendedProperty = new ClockProperty(activeClips[0].Property);
            
            // 重みの合計を計算
            float totalWeight = activeClips.Sum(clip => clip.Weight);
            
            // 重みの合計が0の場合は最初のプロパティを返す
            if (totalWeight <= 0)
            {
                return activeClips[0].Property;
            }
            
            // 各プロパティの値をリセット
            float bpm = 0f;
            float bpmScale = 0f;
            float offsetTime = 0f;
            float staggerDelay = 0f;
            
            // 最も重みの大きいクリップを見つける（非数値プロパティ用）
            var maxWeightClip = activeClips.OrderByDescending(c => c.Weight).First();
            
            // 各クリップの寄与を計算
            foreach (var clip in activeClips)
            {
                float normalizedWeight = clip.Weight / totalWeight;
                
                // 数値プロパティは線形補間
                bpm += clip.Property.bpm.value * normalizedWeight;
                bpmScale += clip.Property.bpmScale.value * normalizedWeight;
                offsetTime += clip.Property.offsetTime.value * normalizedWeight;
                staggerDelay += clip.Property.staggerDelay.value * normalizedWeight;
            }
            
            // ブレンドした値を設定
            blendedProperty.bpm.value = bpm;
            blendedProperty.bpmScale.value = bpmScale;
            blendedProperty.offsetTime.value = offsetTime;
            blendedProperty.staggerDelay.value = staggerDelay;
            
            // 非数値プロパティは最も重みの大きいクリップのものを使用
            blendedProperty.loopType.value = maxWeightClip.Property.loopType.value;
            blendedProperty.arrayStaggerValue = maxWeightClip.Property.arrayStaggerValue;
            blendedProperty.clipProperty = maxWeightClip.Property.clipProperty;
            
            Debug.Log($"StageLightMasterClockMixer: Linear Blend - BPM = {blendedProperty.bpm.value}, Clips: {activeClips.Count}, Weights: {string.Join(", ", activeClips.Select(c => c.Weight))}");
            
            return blendedProperty;
        }
    }
}