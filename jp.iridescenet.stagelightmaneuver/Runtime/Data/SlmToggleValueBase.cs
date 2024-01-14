using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;


namespace StageLightManeuver
{

    [System.AttributeUsage(System.AttributeTargets.Field)]
    public class SlmValueAttribute : PropertyAttribute
    {
        public readonly string? name;
        public readonly bool isHidden;
        public SlmValueAttribute(string? name = null, bool isHidden = false)
        {
            this.name = name;
            this.isHidden = isHidden;
        }
    }

    [System.AttributeUsage(System.AttributeTargets.Class)]
    public class SlmPropertyAttribute : PropertyAttribute
    {
        public readonly bool isRemovable;
        public SlmPropertyAttribute(bool isRemovable = true)
        {
            this.isRemovable = isRemovable;
        }
    }
    
    
    [Serializable]
    public class SlmToggleValueBase
    {
        [SerializeField, SlmValue(isHidden: true)] public bool propertyOverride = false;
        [SlmValue(isHidden: true)] public int sortOrder = 0;
    }

    [Serializable]
    public class SlmToggleValue<T>:SlmToggleValueBase
    {
        public T value;
        public SlmToggleValue(SlmToggleValue<T> slmToggleValue)
        {
            propertyOverride = slmToggleValue.propertyOverride;
            this.value = slmToggleValue.value;
        }

        public SlmToggleValue()
        {
            propertyOverride = false;
            value = default;
        }
    }


    [Serializable]
    public enum StageLightPropertyType
    {
        None,
        Array,
    }

    [Serializable]
    public class SlmProperty:SlmToggleValueBase
    {
        [SlmValue(isHidden: true)] public StageLightPropertyType propertyType = StageLightPropertyType.None;
        [SlmValue(isHidden: true)] public string propertyName;
        [SlmValue(isHidden: true)] public int propertyOrder = 0;
        [SlmValue(isHidden: true)] public bool isEditable = true;
        public virtual void ToggleOverride(bool toggle)
        {
            propertyOverride = toggle;
        }

        public virtual void OnProcessFrame(float time, float clipStart, float clipDuration)
        {
            
        }
        
        public virtual void OverwriteProperty(SlmProperty other)
        {
        }
        
        public virtual void InitStageLightSupervisor(StageLightUniverse stageLightUniverse)
        {
        }

    }
    
    
    [Serializable]
      public class ClockOverride
    {
        [SlmValue("Loop Type")] public LoopType loopType = LoopType.Loop;
        [SlmValue("Offset Time")] public float offsetTime = 0f;
        [SlmValue("BPM Scale")]public float bpmScale = 1f;
        [SlmValue("Child Stagger")]public float childStagger = 0f;
        public ArrayStaggerValue arrayStaggerValue = new ArrayStaggerValue();

        public ClockOverride()
        {
            loopType = LoopType.Loop;
            bpmScale = 1f;
            offsetTime = 0f;
            childStagger = 0f;
            arrayStaggerValue = new ArrayStaggerValue();
        }
        
        public ClockOverride(ClockOverride clockOverride)
        {
            loopType = clockOverride.loopType;
            bpmScale = clockOverride.bpmScale;
            offsetTime = clockOverride.offsetTime;
            childStagger = clockOverride.childStagger;
            arrayStaggerValue = new ArrayStaggerValue(clockOverride.arrayStaggerValue);
        }
        
       
        
        
    }
      
      public interface IArrayProperty
      {
          // void ResyncArraySize(StageLightUniverse stageLightUniverse);
          public void ResyncArraySize(List<StageLightFixture> stageLights);
      } 
    
    [Serializable]
    public class SlmAdditionalProperty:SlmProperty,IArrayProperty
    {
        
        public SlmToggleValue<ClockOverride> clockOverride = new  SlmToggleValue<ClockOverride>()
        {
            value = new ClockOverride(),
            sortOrder = -999,
        };
        
        public SlmAdditionalProperty()
        {
            propertyType = StageLightPropertyType.Array;
            propertyOverride = true;
        }

        public virtual void ResyncArraySize(List<StageLightFixture> stageLights)
        {
            if(clockOverride.value != null && clockOverride.value.arrayStaggerValue != null)
                clockOverride.value.arrayStaggerValue.ResyncArraySize(stageLights);
        }
    }

    [Serializable]
    public class SlmBarLightProperty : SlmAdditionalProperty
    {
        public virtual void ResizeBarLightArray(List<LightChannel> lightChannels)
        {
        }
    }
 
    
    // [Serializable]
    // public class SlmArrayProperty:SlmProperty,IArrayProperty
    // {
    //     public SlmToggleValue<ClockOverride> clockOverride = new  SlmToggleValue<ClockOverride>()
    //     {
    //         sortOrder = -999
    //     };
    //     public virtual void ResyncArraySize(StageLightUniverse stageLightUniverse)
    //     {
    //         
    //     }
    // }
    //
   
    
    
    [Serializable]
    public class ClipProperty
    {
        public float clipStartTime;
        public float clipEndTime;
        
        public ClipProperty()
        {
            clipStartTime = 0f;
            clipEndTime = 0f;
        }

        public ClipProperty(ClipProperty other)
        {
            clipStartTime = other.clipStartTime;
            clipEndTime = other.clipEndTime;
        }
    }







}