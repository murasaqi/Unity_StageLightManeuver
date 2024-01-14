using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace StageLightManeuver
{

    [Serializable]
    public enum StaggerCalculationType
    {
        StaggerIn,
        StaggerOut,
        StaggerInOut,
        Random,
        Manual,
    }
        [Serializable]
    public class ArrayStaggerValue:IArrayProperty
    {
        public StaggerCalculationType staggerCalculationType = StaggerCalculationType.StaggerInOut;
       
        public float animationDuration = 1f;
        // [FormerlySerializedAs("staticDuration")] public float animationDuration = 1f;
        [FormerlySerializedAs("staticDelay")] [Range(0,1)]public float delayRatio = 0.1f;
        public AnimationCurve animationCurve = AnimationCurve.EaseInOut(0,0,1,1);
        public List<Vector2> lightStaggerInfo = new List<Vector2>();
        
        public List<Vector2> randomStaggerInfo = new List<Vector2>();

        public ArrayStaggerValue()
        {
            staggerCalculationType = StaggerCalculationType.StaggerInOut;
            animationDuration = 1f;
            delayRatio = 0.1f;
            animationCurve = AnimationCurve.EaseInOut(0,0,1,1);
            lightStaggerInfo = new List<Vector2>();
            randomStaggerInfo = new List<Vector2>();
        }
        
        public ArrayStaggerValue(ArrayStaggerValue arrayStaggerValue)
        {
            staggerCalculationType = arrayStaggerValue.staggerCalculationType;
            animationDuration = arrayStaggerValue.animationDuration;
            delayRatio = arrayStaggerValue.delayRatio;
            animationCurve = SlmUtility.CopyAnimationCurve(arrayStaggerValue.animationCurve);
            lightStaggerInfo = new List<Vector2>(arrayStaggerValue.lightStaggerInfo);
            randomStaggerInfo = new List<Vector2>(arrayStaggerValue.randomStaggerInfo);
        }
        public void ResyncArraySize(List<StageLightFixture> stageLights)
        {
            var countDifference = stageLights.Count - lightStaggerInfo.Count;
            if (countDifference > 0)
            {
                for (int i = 0; i < countDifference; i++)
                {
                    lightStaggerInfo.Add(new Vector2(0, 1));
                }
            }
            else if (countDifference < 0)
            {
                lightStaggerInfo.RemoveRange(lightStaggerInfo.Count + countDifference, -countDifference);
            }
            
            countDifference = stageLights.Count - randomStaggerInfo.Count;
            if (countDifference > 0)
            {
                for (int i = 0; i < countDifference; i++)
                {
                    randomStaggerInfo.Add(new Vector2(0, 1));
                }
            }
            else if (countDifference < 0)
            {
                randomStaggerInfo.RemoveRange(randomStaggerInfo.Count + countDifference, -countDifference);
            }
            CalculateStaggerTime();
        } 
        public void CalculateStaggerTime()
        {
            if(staggerCalculationType == StaggerCalculationType.Manual) return;
            if (delayRatio >= 1)
            {
                delayRatio = 0.99f;
            }
            
           
            if( staggerCalculationType == StaggerCalculationType.StaggerIn)
            {
                var delayStep = delayRatio / (lightStaggerInfo.Count-1);    
                
                for (int i = 0; i < lightStaggerInfo.Count; i++)
                {
                    var delay = delayStep * i;
                    lightStaggerInfo[i] = new Vector2(delay, 1f);
                }
            } else if( staggerCalculationType == StaggerCalculationType.StaggerOut)
            {
                var delayStep = delayRatio / (lightStaggerInfo.Count-1);
                for (int i = 0; i < lightStaggerInfo.Count; i++)
                {
                    var delay = animationDuration-delayStep * i;
                    lightStaggerInfo[i] = new Vector2(0f, delay);
                }
            }else if (staggerCalculationType == StaggerCalculationType.StaggerInOut)
            {
                var duration = animationDuration*(1-delayRatio);
                for (int i = 0; i < lightStaggerInfo.Count; i++)
                {
                    var delayStep = delayRatio / (lightStaggerInfo.Count-1);
                    var delay = (delayStep) * i;
                    lightStaggerInfo[i] = new Vector2(delay, delay+duration);
                }
            }

            
        }
        public void CalculateRandomStaggerTime()
        {
            if(staggerCalculationType != StaggerCalculationType.Random) return;
            for (int i = 0; i < randomStaggerInfo.Count; i++)
            {
                var duration = animationDuration * (1 - delayRatio);
                var totalDelay = animationDuration - duration;
                var delay = Random.Range(0f, totalDelay);
                randomStaggerInfo[i] = new Vector2(delay, delay+duration);
            }
        }
        
        public Vector2 GetStaggerStartEnd(int index)
        {
            if(staggerCalculationType == StaggerCalculationType.Random)
            {
                return randomStaggerInfo[index];
            }
            return lightStaggerInfo[index];
        }
        
        public float Evaluate(float normalizedTime,int index)
        {
            var staggerStartEnd = GetStaggerStartEnd(index);
            var progress = Mathf.InverseLerp(staggerStartEnd.x, staggerStartEnd.y, normalizedTime);
            var resut = animationCurve.Evaluate(progress);
            return resut;

        }
    }
  
    [Serializable, SlmProperty(isRemovable: false)]
    public class ClockProperty: SlmProperty,IArrayProperty
    {
        [SlmValue("Clip Duration", true)] public ClipProperty clipProperty;
        [SlmValue("Loop Type")] public SlmToggleValue<LoopType> loopType;
        [SlmValue("BPM")]public SlmToggleValue<float> bpm;
        [SlmValue("BPM Scale")]public SlmToggleValue<float> bpmScale;
        [SlmValue("Offset Time")] public SlmToggleValue<float> offsetTime;
        [FormerlySerializedAs("childStagger")] [SlmValue("Child Stagger")]public SlmToggleValue<float> staggerDelay;
        public ArrayStaggerValue arrayStaggerValue = new ArrayStaggerValue();

        public ClockProperty()
        {
            propertyName = "Clock";
            propertyOrder = -999;
            propertyOverride = true;
            loopType = new SlmToggleValue<LoopType>(){value = LoopType.Loop, propertyOverride = true};
            clipProperty = new ClipProperty(){clipStartTime = 0, clipEndTime = 0};
            bpm = new SlmToggleValue<float>() { value = 60 , propertyOverride = true};
            bpmScale = new SlmToggleValue<float>() { value = 1f , propertyOverride = true};
            staggerDelay = new SlmToggleValue<float>() { value = 0f , propertyOverride = false};
            offsetTime = new SlmToggleValue<float>() { value = 0f , propertyOverride = true};
        }

        public void ResyncArraySize(List<StageLightFixture> stageLights)
        {
            arrayStaggerValue.ResyncArraySize(stageLights);
        }
        public override void ToggleOverride(bool toggle)
        {
            base.ToggleOverride(toggle);
            propertyOverride = toggle;
            loopType.propertyOverride = toggle;
            bpm.propertyOverride = toggle;
            bpmScale.propertyOverride = toggle;
            // staggerDelay.propertyOverride = toggle;
            offsetTime.propertyOverride = toggle;
            
        }

        public ClockProperty(ClockProperty other)
        {
            propertyOverride = other.propertyOverride;
            propertyName = other.propertyName;
            bpm = new SlmToggleValue<float>(other.bpm);
            bpmScale = new SlmToggleValue<float>(other.bpmScale);
            staggerDelay = new SlmToggleValue<float>(other.staggerDelay);
            loopType = new SlmToggleValue<LoopType>(other.loopType);
            clipProperty = new ClipProperty(other.clipProperty);
            offsetTime = new SlmToggleValue<float>(other.offsetTime);
            
        }

        public override void OverwriteProperty(SlmProperty other)
        {
            if (other is ClockProperty timeProperty)
            {
                if (timeProperty.propertyOverride)
                {
                    propertyOverride = timeProperty.propertyOverride;
                    if(timeProperty.bpm.propertyOverride) bpm.value = timeProperty.bpm.value;
                    if(timeProperty.bpmScale.propertyOverride) bpmScale.value = timeProperty.bpmScale.value;
                    // if(timeProperty.staggerDelay.propertyOverride) staggerDelay.value = timeProperty.staggerDelay.value;
                    if(timeProperty.loopType.propertyOverride) loopType.value = timeProperty.loopType.value;
                    if(timeProperty.offsetTime.propertyOverride) offsetTime.value = timeProperty.offsetTime.value;
                }
            }
        }
    }
}