using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.SceneManagement;

// using System.Reflection;

namespace StageLightManeuver
{
    
    public static class SlmUtility
    {


        public static string BaseExportPath = SlmSettingsUtility.BaseExportProfilePath;

        public static string GetExportPath(string path, string clipName)
        {
            return path.Replace("<Scene>", SceneManager.GetActiveScene().name).Replace("<ClipName>", clipName);
        }

        public static float GetOffsetTime(float currentTime, StageLightQueueData queData, Type propertyType,int index)
        {
            var additionalProperty = queData.TryGetActiveProperty(propertyType) as SlmAdditionalProperty;
            var clockProperty = queData.TryGetActiveProperty<ClockProperty>();
            var bpm =clockProperty.bpm.value;
            var staggerDelay = additionalProperty.clockOverride.propertyOverride ? additionalProperty.clockOverride.value.childStagger : clockProperty.staggerDelay.value;
            var bpmScale = additionalProperty.clockOverride.propertyOverride ? additionalProperty.clockOverride.value.bpmScale : clockProperty.bpmScale.value;
            var offsetTime = additionalProperty.clockOverride.propertyOverride ? additionalProperty.clockOverride.value.offsetTime :clockProperty.offsetTime.value;
           
            var scaledBpm = bpm * bpmScale;
            var duration = 60 / scaledBpm;
            var offset = duration* staggerDelay * (index+1);
            // var offsetTime = time + offset;
            
            
            return currentTime+offset;
        }
        
        public static float GetNormalizedTime(float currentTime, ClockProperty clockOverride, SlmAdditionalProperty slmAdditionalProperty)
        {
            
            var bpmOverrideData = slmAdditionalProperty.clockOverride;
            var offsetTime = bpmOverrideData.propertyOverride ? bpmOverrideData.value.offsetTime : clockOverride.offsetTime.value;
            var bpm =  clockOverride.bpm.value;
            if(clockOverride.bpmScale.value <= 0) clockOverride.bpmScale.value = 1;
            if(clockOverride.bpm.value <= 0) clockOverride.bpm.value = 120;
            if(slmAdditionalProperty.clockOverride.value.bpmScale <= 0) slmAdditionalProperty.clockOverride.value.bpmScale = 1;
            var stagger =bpmOverrideData.propertyOverride ? bpmOverrideData.value.childStagger : clockOverride.staggerDelay.value;
            var bpmScale = bpmOverrideData.propertyOverride ? bpmOverrideData.value.bpmScale : clockOverride.bpmScale.value;
            var loopType = bpmOverrideData.propertyOverride ? bpmOverrideData.value.loopType : clockOverride.loopType.value;
            
            var arrayStaggerValue = bpmOverrideData.propertyOverride ? bpmOverrideData.value.arrayStaggerValue: clockOverride.arrayStaggerValue;
            return SlmUtility.GetNormalizedTime(currentTime+offsetTime, bpm, stagger,bpmScale,clockOverride.clipProperty, loopType,arrayStaggerValue);
        }

        public static List<SlmProperty> CopyProperties(StageLightClipProfile referenceStageLightClipProfile)
        {
            // プロファイルのコピーを作成
            var profile = UnityEngine.Object.Instantiate(referenceStageLightClipProfile);
            var copyList = new List<SlmProperty>();
            foreach (var stageLightProperty in profile.stageLightProperties)
            {
                if(stageLightProperty == null) continue;

                var type = stageLightProperty.GetType();
                var copy = Activator.CreateInstance(type, BindingFlags.CreateInstance, null,
                        new object[] { stageLightProperty }, null)
                    as SlmProperty;
                copy.isEditable = true;
                copyList.Add(copy);
            }
            // プロファイルのコピーを破棄
            UnityEngine.Object.DestroyImmediate(profile);

            var timeProperty = copyList.Find(x => x.GetType() == typeof(ClockProperty));

            if (timeProperty == null)
            {
                copyList.Insert(0, new ClockProperty());
            }
            
            var orderProperty = copyList.Find(x => x.GetType() == typeof(StageLightOrderProperty));
            if(orderProperty == null)
            {
                copyList.Insert(1, new StageLightOrderProperty());
            }

            return copyList;
        }

        public static float GetNormalizedTime(float time, StageLightQueueData queData, Type propertyType, int index = 0)
        {

            var additionalProperty = queData.TryGetActiveProperty(propertyType);
            var clockProperty = queData.TryGetActiveProperty<ClockProperty>();
            var weight = queData.weight;
            if (additionalProperty == null || clockProperty == null) return 0f;
            if (clockProperty.bpm.value <= 0) clockProperty.bpm.value = 120;
            if(clockProperty.bpmScale.value <= 0) clockProperty.bpmScale.value = 1;
            if(additionalProperty.clockOverride.value.bpmScale <= 0) additionalProperty.clockOverride.value.bpmScale = 1;
            var bpm = clockProperty.bpm.value;
            var stagger = additionalProperty.clockOverride.propertyOverride ? additionalProperty.clockOverride.value.childStagger : clockProperty.staggerDelay.value;
            var bpmScale = additionalProperty.clockOverride.propertyOverride ? additionalProperty.clockOverride.value.bpmScale : clockProperty.bpmScale.value;

            var loopType = additionalProperty.clockOverride.propertyOverride ? additionalProperty.clockOverride.value.loopType : clockProperty.loopType.value;
            var offsetTime = additionalProperty.clockOverride.propertyOverride
              ? additionalProperty.clockOverride.value.offsetTime
              : clockProperty.offsetTime.value;
            var clipProperty = clockProperty.clipProperty;
            var arrayStaggerValue = additionalProperty.clockOverride.propertyOverride ? additionalProperty.clockOverride.value.arrayStaggerValue : clockProperty.arrayStaggerValue;
            var t = GetNormalizedTime(time+offsetTime,bpm,stagger,bpmScale,clipProperty,loopType,arrayStaggerValue,index);
            return t; 
        }

        
        public static List<StageLightClipProfile> GetProfileInProject()
        {
#if UNITY_EDITOR
            var guids = AssetDatabase.FindAssets("t:StageLightProfile");
              var profiles = new List<StageLightClipProfile>();
              foreach (var guid in guids)
              {
                  var path = AssetDatabase.GUIDToAssetPath(guid);
                  var profile = AssetDatabase.LoadAssetAtPath<StageLightClipProfile>(path);
                  profiles.Add(profile);
              }
              
            return profiles;
#else
        return null;
#endif
        }
        
        public static float GetNormalizedTime(float time,float bpm, float bpmOffset,float bpmScale,ClipProperty clipProperty,LoopType loopType, ArrayStaggerValue arrayStaggerValue, int index = 0)
        {
            
            var scaledBpm = bpm * bpmScale;
            var duration = 60 / scaledBpm;
            var offset = duration* bpmOffset * (index+1);
            var offsetTime = time + offset;
             var result = 0f;
            var t = (float)offsetTime % duration;
            var normalisedTime = t / duration;
            
            if (loopType == LoopType.Loop)
            {
                result = normalisedTime;     
            }else if (loopType == LoopType.PingPong)
            {
                result = Mathf.PingPong(offsetTime / duration, 1f);
            }
            else if(loopType == LoopType.Fixed)
            {
                result = Mathf.InverseLerp(clipProperty.clipStartTime, clipProperty.clipEndTime, time);
            }
            else if(loopType == LoopType.FixedStagger)
            {
                result = arrayStaggerValue.Evaluate( Mathf.InverseLerp(clipProperty.clipStartTime, clipProperty.clipEndTime, time), index);
            }
           
            return result;
        }

        
        public static Color GetHDRColor(Color color, float intensity)
        {
            return new Color(color.r, color.g, color.b, color.a) *Mathf.Pow(2.0f,intensity);
        }
        
        public static AnimationCurve CopyAnimationCurve(AnimationCurve curve)
        {
            var newCurve = new AnimationCurve();
            var copyKeys = new Keyframe[curve.keys.Length];
            curve.keys.CopyTo(copyKeys, 0);
            newCurve.keys = copyKeys;
            newCurve.preWrapMode = curve.preWrapMode;
            newCurve.postWrapMode = curve.postWrapMode;
            return newCurve;
        }
        public static Gradient CopyGradient(Gradient gradient)
        {
            Gradient newGradient = new Gradient();
            var colorKeys = new GradientColorKey[gradient.colorKeys.Length];
            var alphaKeys = new GradientAlphaKey[gradient.alphaKeys.Length];
            
            for (int i = 0; i < gradient.colorKeys.Length; i++)
            {
                colorKeys[i] = gradient.colorKeys[i];
            }
            
            for (int i = 0; i < gradient.alphaKeys.Length; i++)
            {
                alphaKeys[i] = gradient.alphaKeys[i];
            }
            newGradient.SetKeys(colorKeys, alphaKeys);
            newGradient.mode = gradient.mode;
            
            return newGradient;
        }
    }
}