using System;
using System.Collections.Generic;
using UnityEngine;

namespace StageLightManeuver
{
    
   
    [Serializable]
    public class RollProperty:SlmAdditionalProperty
    {
        [SlmValue("Roll Transform")] public SlmToggleValue<MinMaxEasingValue> rollTransform; 
        [SlmValue("Smooth Time")] public SlmToggleValue<float> smoothTime = new SlmToggleValue<float>(); 
        public SlmToggleValue<bool> useSmoothness = new SlmToggleValue<bool>();
        public RollProperty(RollProperty rollProperty)
        {
            propertyName = rollProperty.propertyName;

            clockOverride = new SlmToggleValue<ClockOverride>()
            {
                propertyOverride = rollProperty.clockOverride.propertyOverride,
                value = new ClockOverride(rollProperty.clockOverride.value)
            };
            this.rollTransform = new SlmToggleValue<MinMaxEasingValue>()
            {
                propertyOverride =  rollProperty.rollTransform.propertyOverride,
                value =     new MinMaxEasingValue(rollProperty.rollTransform.value),
            };
            smoothTime = new SlmToggleValue<float>()
            {
                propertyOverride = rollProperty.smoothTime.propertyOverride,
                value = rollProperty.smoothTime.value
            };
            useSmoothness = new SlmToggleValue<bool>()
            {
                propertyOverride = rollProperty.useSmoothness.propertyOverride,
                value = rollProperty.useSmoothness.value
            };
            propertyOverride = rollProperty.propertyOverride;

        }

        public RollProperty()
        {
            propertyOverride = false;
            clockOverride = new SlmToggleValue<ClockOverride>();
            rollTransform = new SlmToggleValue<MinMaxEasingValue>() {value = new MinMaxEasingValue()};
            smoothTime = new SlmToggleValue<float>() {value = 0.05f};
            useSmoothness = new SlmToggleValue<bool>() {value = false};
        }

        public override void ToggleOverride(bool toggle)
        {
            base.ToggleOverride(toggle);
            propertyOverride = toggle;
            rollTransform.propertyOverride = toggle;
            clockOverride.propertyOverride = toggle;
            smoothTime.propertyOverride = toggle;
            useSmoothness.propertyOverride = toggle;
            
        }

        public override void OverwriteProperty(SlmProperty other)
        {
            base.OverwriteProperty(other);
            RollProperty rollProperty = other as RollProperty;
            if (rollProperty == null) return;
            if(rollProperty.rollTransform.propertyOverride) rollTransform.value = new MinMaxEasingValue(rollProperty.rollTransform.value);
            if(rollProperty.clockOverride.propertyOverride) clockOverride = new SlmToggleValue<ClockOverride>(rollProperty.clockOverride);
            if(rollProperty.smoothTime.propertyOverride) smoothTime.value = rollProperty.smoothTime.value;
            if(rollProperty.useSmoothness.propertyOverride) useSmoothness.value = rollProperty.useSmoothness.value;
        }
    }
    
    [Serializable]
    public class PanProperty:RollProperty
    {
        public PanProperty(PanProperty panProperty):base(panProperty)
        {
            propertyName = "Pan";
        }

        public PanProperty():base()
        {
            propertyName = "Pan";
        }
    }
    [Serializable]
    public class TiltProperty:RollProperty
    {
        public TiltProperty(TiltProperty tiltProperty):base(tiltProperty)
        {
            
            propertyName = "Tilt";
        }

        public TiltProperty():base()
        {
            
            propertyName = "Tilt";
        }
    }

}