using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Timeline;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace StageLightManeuver
{
    [CustomTimelineEditor(typeof(StageLightTimelineClip))]

    public class StageLightTimelineClipEditor:ClipEditor
    {
         [InitializeOnLoad]
        class EditorInitialize
        {
            static EditorInitialize()
            {
                backgroundTexture = new Texture2D(1, 1);
                syncIconTexture = Resources.Load<Texture2D>("Icons/icon_sync");
                backgroundTexture.SetPixel(0, 0, Color.white);
                backgroundTexture.Apply();
           
            } 
            static PlayableDirector GetMasterDirector() { return TimelineEditor.masterDirector; }
        }
        Dictionary<StageLightTimelineClip, Texture2D> _gradientTextures = new Dictionary<StageLightTimelineClip, Texture2D>();
        // Dictionary<StageLightTimelineClip, Texture2D> _beatTextures = new Dictionary<StageLightTimelineClip, Texture2D>();

        private Dictionary<StageLightTimelineClip, List<float>>
            _beatPoint = new Dictionary<StageLightTimelineClip, List<float>>();
        private static Texture2D backgroundTexture;
        private static Texture2D syncIconTexture;
     
        public override ClipDrawOptions GetClipOptions(TimelineClip clip)
        {
            return new ClipDrawOptions
            {
                errorText = GetErrorText(clip),
                highlightColor = GetDefaultHighlightColor(clip),
                icons = Enumerable.Empty<Texture2D>(),
                tooltip = "Tooltip"
            };
        }
        
        
        public override void OnClipChanged(TimelineClip clip)
        {
        
            var stageLightTimelineClip = (StageLightTimelineClip)clip.asset;
            if (stageLightTimelineClip == null)
                return;
            GetGradientTexture(clip, true);
            if (stageLightTimelineClip.referenceStageLightClipProfile != null && stageLightTimelineClip.syncClipName)
                clip.displayName = stageLightTimelineClip.referenceStageLightClipProfile.name;
            
            stageLightTimelineClip.clipDisplayName = clip.displayName;
        }
        public override void OnCreate(TimelineClip clip, TrackAsset track, TimelineClip clonedFrom)
        {
            base.OnCreate(clip, track, clonedFrom);
            var stageLightTimelineClip = (StageLightTimelineClip)clip.asset;
            if (stageLightTimelineClip == null)
                return;
            var guids = AssetDatabase.FindAssets( "t:StageLightManeuverSettings" );
            // Debug.Log(guids);
            if (guids.Length > 0)
            {
                var stageLightManeuverSettingsPath = AssetDatabase.GUIDToAssetPath(guids[0]);
                var stageLightManeuverSettingsAsset = AssetDatabase.LoadAssetAtPath<StageLightManeuverSettings>(stageLightManeuverSettingsPath);
                stageLightTimelineClip.exportPath = stageLightManeuverSettingsAsset.exportProfilePath;
            }
            else
            {
                stageLightTimelineClip.exportPath = SlmUtility.BaseExportPath;
            }

       
        }


        public override void DrawBackground(TimelineClip clip, ClipBackgroundRegion region)
        {
            base.DrawBackground(clip, region);
            var stageLightTimelineClip = (StageLightTimelineClip) clip.asset;



            if (stageLightTimelineClip.clipDisplayName != clip.displayName)
            {
                stageLightTimelineClip.clipDisplayName = clip.displayName;
            }
            if (stageLightTimelineClip.track.drawCustomClip)
            {

                if (syncIconTexture == null) syncIconTexture = Resources.Load<Texture2D>("Icons/icon_sync");
                var update = stageLightTimelineClip.forceTimelineClipUpdate;
                if (stageLightTimelineClip.referenceStageLightClipProfile != null)
                {
                    if (stageLightTimelineClip.referenceStageLightClipProfile.isUpdateGuiFlag) update = true;
                    stageLightTimelineClip.referenceStageLightClipProfile.isUpdateGuiFlag = false;
                }

                var tex = GetGradientTexture(clip, update);
                if (stageLightTimelineClip.track == null) return;
                var colorHeight = region.position.height * stageLightTimelineClip.track.colorLineHeight;
                // var beatHeight = 2f;

                UpdateBeatPoint(clip);

                var duration = region.endTime - region.startTime;
                var width = (float)(region.position.width * clip.duration / duration);
                var left = Mathf.Max((float)clip.clipIn, (float)region.startTime);
                var offset = (float)(region.position.width * left / duration);
                var start = region.position.x - offset;
                if (_beatPoint.ContainsKey(stageLightTimelineClip))
                {
                    foreach (var p in _beatPoint[stageLightTimelineClip])
                    {

                        EditorGUI.DrawRect(new Rect(start + width * p, 0, 1, region.position.height),
                            stageLightTimelineClip.track.beatLineColor);
                    }


                }

                var size = 10;
                for (float i = 0; i < size; i++)
                {

                    // var with = region.position.width;
                    // EditorGUI.DrawRect(new Rect(region.position.x + with*, 0, 1, region.position.height),
                    //     stageLightTimelineClip.track.beatLineColor);
                }

                if (tex)
                {
                    GUI.DrawTexture(
                        new Rect(start, region.position.height - colorHeight, width,
                            colorHeight), tex);
                }


                var iconSize = 12;
                var margin = 4;
                if (syncIconTexture && stageLightTimelineClip.referenceStageLightClipProfile != null &&
                    stageLightTimelineClip.syncReferenceProfile)
                {
                    GUI.DrawTexture(new Rect(start+width - iconSize - margin, margin, iconSize, iconSize),
                        syncIconTexture, ScaleMode.ScaleAndCrop,
                        true,
                        0,
                        new Color(1, 1, 1, 0.5f), 0, 0);

                }
            }

            stageLightTimelineClip.forceTimelineClipUpdate = false;
        }
        
        public void UpdateBeatPoint(TimelineClip clip,float step = 0.01f)
        {
            var customClip = clip.asset as StageLightTimelineClip;
            var beatPointList = new List<float>();

            if (customClip.StageLightQueueData == null)
            {
                customClip.InitStageLightProfile();
            }
            
            var timeProperty = customClip.StageLightQueueData.TryGetActiveProperty<ClockProperty>();
            
            if (timeProperty != null)
            {
                var preBeat = -1f;
                for (float i =(float) clip.start; i < (float)clip.end; i+=step)
                {
                    var bpm = timeProperty.bpm.value;
                    var bpmOffset =timeProperty.staggerDelay.value;
                    var bpmScale = timeProperty.bpmScale.value;
                    var loopType = LoopType.Loop;
                    var arrayStaggerValue = timeProperty.arrayStaggerValue;
                    var t =SlmUtility.GetNormalizedTime(i,bpm,bpmOffset,bpmScale,timeProperty.clipProperty,loopType,arrayStaggerValue);
                    
                    
                    if (preBeat> t)
                    {
                        var point = (float)(i - clip.start);
                        beatPointList.Add(point/(float)clip.duration );
                    }
                    
                    preBeat = t;
                }
            }
            
            if (_beatPoint.ContainsKey(customClip))
            {
                _beatPoint[customClip] = beatPointList;
            }
            else
            {
                _beatPoint.Add(customClip, beatPointList);    
            }

        }
        
        Texture2D GetGradientTexture(TimelineClip clip, bool update = false)
        {
            Texture2D tex = Texture2D.whiteTexture;

            var customClip = clip.asset as StageLightTimelineClip;
            
            if (!customClip) return tex;

            if(customClip.StageLightQueueData == null) return tex;
            var lightProperty = customClip.StageLightQueueData.TryGetActiveProperty<LightProperty>();
            var materialColorProperty = customClip.StageLightQueueData.TryGetActiveProperty<MaterialColorProperty>();
            var lightColorProperty = customClip.StageLightQueueData.TryGetActiveProperty<LightColorProperty>();
            if(lightColorProperty == null && lightProperty == null && materialColorProperty == null) return tex;
            
        

        
            var gradient = new Gradient();
            if (lightColorProperty != null && materialColorProperty == null)
            {
                if(lightColorProperty.lightToggleColor == null) return tex;
                gradient = lightColorProperty.lightToggleColor.value;
            }
            else 
            {
                if(materialColorProperty == null || materialColorProperty.color == null) return tex;
                if (materialColorProperty.color.value == null)
                {
                    materialColorProperty.color.value = new Gradient();
                }
                gradient = materialColorProperty.color.value;
            }
           

            var lightIntensityProperty = customClip.StageLightQueueData.TryGetActiveProperty<LightIntensityProperty>();
            if (update) 
            {
                _gradientTextures.Remove(customClip);
            }
            else
            {
                _gradientTextures.TryGetValue(customClip, out tex);
                if (tex) return tex;
            }
            tex = new Texture2D(64, 1);
            var b = (float)(clip.blendInDuration / clip.duration);
          
            for (int i = 0; i < tex.width; ++i)
            {
                var currentTime  =(float)clip.start+(float)clip.duration* (float)i / tex.width;
            
                var timeProperty = customClip.StageLightQueueData.TryGetActiveProperty<ClockProperty>();
                if (timeProperty != null)
                {
                    
                    var color = Color.black;


                    if (lightColorProperty != null)
                    {
                        var lightT = SlmUtility.GetNormalizedTime(currentTime, timeProperty, lightColorProperty);
                        color = gradient.Evaluate(lightT);     
                       
                    }
                    else if(materialColorProperty != null)
                    {
                        var materialT = SlmUtility.GetNormalizedTime(currentTime, timeProperty, materialColorProperty);
                        color = gradient.Evaluate(materialT);
                    }

                    var intensityValue = 1f;
                    if (lightIntensityProperty != null)
                    {
                        var t = SlmUtility.GetNormalizedTime(currentTime, timeProperty, lightIntensityProperty);
                        intensityValue = lightIntensityProperty.lightToggleIntensity.value.Evaluate(t);
                        // intensityValue = intensityValue
                    }
                    else if(materialColorProperty != null)
                    {
                        var t = SlmUtility.GetNormalizedTime(currentTime, timeProperty, materialColorProperty);
                        intensityValue = materialColorProperty.intensity.value.Evaluate(t);
                    }
                    
                    color = new Color(color.r,
                        color.g,
                        color.b,
                        color.a*intensityValue);
                    tex.SetPixel(i, 0, color);     
                    
                }
            }

            
            
            tex.Apply();
            if (_gradientTextures.ContainsKey(customClip))
            {
                _gradientTextures[customClip] = tex;
            }
            else
            {
                _gradientTextures.Add(customClip, tex);    
            }

           
            return tex;
        }

        
        public override void GetSubTimelines(TimelineClip clip, PlayableDirector director, List<PlayableDirector> subTimelines)
        {
            base.GetSubTimelines(clip, director, subTimelines);
        }
    }
    
    
}