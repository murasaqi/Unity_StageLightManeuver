using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace StageLightManeuver
{

    [Serializable]
    public class LightIntensityProperty:SlmAdditionalProperty
    {
        [SlmValue("Intensity")]public SlmToggleValue<MinMaxEasingValue> lightToggleIntensity;// = new StageLightProperty<float>(){value = 1f};
        public LightIntensityProperty()
        {
            propertyOverride = true;
            propertyName = "Intensity";
            clockOverride = new SlmToggleValue<ClockOverride>();
            lightToggleIntensity = new SlmToggleValue<MinMaxEasingValue>(){value = new MinMaxEasingValue()
            {
                minMaxValue = new Vector2(0, 10),
                constant = 1f,
                mode = AnimationMode.Constant
            }};
        }
        
        public override void ToggleOverride(bool toggle)
        {
            propertyOverride = toggle;
            clockOverride.propertyOverride = toggle;
            lightToggleIntensity.propertyOverride = toggle;
        }
        
        public LightIntensityProperty( LightIntensityProperty other )
        {
            propertyName = other.propertyName;
            propertyOverride = other.propertyOverride;
            clockOverride = new SlmToggleValue<ClockOverride>(other.clockOverride);
            lightToggleIntensity = new SlmToggleValue<MinMaxEasingValue>()
            {
                propertyOverride = other.lightToggleIntensity.propertyOverride,
                value = new MinMaxEasingValue(other.lightToggleIntensity.value)
            };
        }

        public override void OverwriteProperty(SlmProperty other)
        {
            if (other is LightIntensityProperty)
            {
                var otherProperty = other as LightIntensityProperty;
                if (other.propertyOverride)
                {
                    if(otherProperty.lightToggleIntensity.propertyOverride) lightToggleIntensity.value = new MinMaxEasingValue(otherProperty.lightToggleIntensity.value);
                    if(otherProperty.clockOverride.propertyOverride) clockOverride = new SlmToggleValue<ClockOverride>(otherProperty.clockOverride);
                }
                    
            }
        }
    }
}