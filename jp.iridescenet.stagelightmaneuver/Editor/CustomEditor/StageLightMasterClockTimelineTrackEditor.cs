using UnityEditor;
using UnityEditor.Timeline;
using UnityEngine;
using UnityEngine.Timeline;
using System.Collections.Generic;

namespace StageLightManeuver
{
    /// <summary>
    /// Clock Track用のカスタムエディタ
    /// </summary>
    [CustomTimelineEditor(typeof(StageLightMasterClockTimelineTrack))]
    public class StageLightMasterClockTimelineTrackEditor : TrackEditor
    {
        /// <summary>
        /// トラックのカスタムオプションを取得
        /// </summary>
        public override TrackDrawOptions GetTrackOptions(TrackAsset track, Object binding)
        {
            var options = base.GetTrackOptions(track, binding);
            
            var clockTrack = track as StageLightMasterClockTimelineTrack;
            if (clockTrack != null && clockTrack.affectAllTracksBelow)
            {
                // 影響範囲を持つトラックの色を変更
                options.trackColor = new Color(0.5f, 0.8f, 0.8f);
                // trackHeaderColorプロパティは存在しないため削除
            }
            
            return options;
        }
        
        /// <summary>
        /// トラックGUIの描画後に呼ばれるメソッド
        /// </summary>
        public override void OnTrackChanged(TrackAsset track)
        {
            base.OnTrackChanged(track);
            
            var clockTrack = track as StageLightMasterClockTimelineTrack;
            if (clockTrack == null) return;
            
            // トラックが変更された時の処理
        }
        
        /// <summary>
        /// カスタムGUIを描画するためのメソッド
        /// </summary>
        private void DrawInfluenceIndicator(Rect trackRect, StageLightMasterClockTimelineTrack stageLightMasterClockTrack)
        {
            if (stageLightMasterClockTrack.affectAllTracksBelow)
            {
                // 影響範囲を示す視覚的な表示を追加
                Color originalColor = GUI.color;
                GUI.color = new Color(0.5f, 0.8f, 0.8f, 0.2f);
                
                // トラックの下に影響範囲を示す矩形を描画
                Rect influenceRect = trackRect;
                influenceRect.y += trackRect.height;
                influenceRect.height = 5f; // 影響範囲の高さ
                
                GUI.Box(influenceRect, "");
                
                GUI.color = originalColor;
            }
        }
    }
}