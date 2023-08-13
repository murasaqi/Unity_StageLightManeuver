using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace StageLightManeuver
{

    [Serializable]
    public class BarLightIntensityProperty:SlmAdditionalProperty
    {   
        [SlmValue("Intensity")]public SlmToggleValue<MinMaxEasingValue> intensity;
        public SlmToggleValue<AnimationCurve> arrayLightIntensityMultiplier;
        // public SlmToggleValue<float> barLightIntensityMin
        public BarLightIntensityProperty()
        {
            propertyName = "Bar Light Intensity";
            clockOverride = new SlmToggleValue<ClockOverride>();
            
            intensity = new SlmToggleValue<MinMaxEasingValue>(){value = new MinMaxEasingValue()
            {
                minMaxValue = new Vector2(0, 10),
                constant = 1f,
                mode = AnimationMode.Constant
            }};
            
            arrayLightIntensityMultiplier = new SlmToggleValue<AnimationCurve>(){value = AnimationCurve.Linear(0, 1, 1, 1)};
        }
        
        public override void ToggleOverride(bool toggle)
        {
            propertyOverride = toggle;
            clockOverride.propertyOverride = toggle;
            intensity.propertyOverride = toggle;
            arrayLightIntensityMultiplier.propertyOverride = toggle;
        }
        
        public BarLightIntensityProperty( BarLightIntensityProperty other )
        {
            propertyName = other.propertyName;
            clockOverride = new SlmToggleValue<ClockOverride>(other.clockOverride);
            intensity = new SlmToggleValue<MinMaxEasingValue>()
            {
                propertyOverride = other.intensity.propertyOverride,
                value = new MinMaxEasingValue(other.intensity.value)
            };
            arrayLightIntensityMultiplier = new SlmToggleValue<AnimationCurve>()
            {
                propertyOverride = other.arrayLightIntensityMultiplier.propertyOverride,
                value =SlmUtility.CopyAnimationCurve(other.arrayLightIntensityMultiplier.value)
            };
            
            
        }

        public override void OverwriteProperty(SlmProperty other)
        {
            if (other is BarLightIntensityProperty)
            {
                var otherProperty = other as BarLightIntensityProperty;
                if (other.propertyOverride)
                {
                    if(otherProperty.intensity.propertyOverride) intensity.value = new MinMaxEasingValue(otherProperty.intensity.value);
                    if(otherProperty.clockOverride.propertyOverride) clockOverride = new SlmToggleValue<ClockOverride>(otherProperty.clockOverride);
                    if(otherProperty.arrayLightIntensityMultiplier.propertyOverride) arrayLightIntensityMultiplier = new SlmToggleValue<AnimationCurve>()
                    {
                        propertyOverride = otherProperty.arrayLightIntensityMultiplier.propertyOverride,
                        value =SlmUtility.CopyAnimationCurve(otherProperty.arrayLightIntensityMultiplier.value)
                    };
                }
                
                    
            }
        }
    }
}