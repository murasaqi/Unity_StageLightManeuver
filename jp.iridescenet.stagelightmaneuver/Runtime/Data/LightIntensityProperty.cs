using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace StageLightManeuver
{

    [Serializable]
    public class LightIntensityProperty:SlmAdditionalProperty
    {
         [FormerlySerializedAs("lightToggleIntensity")] [SlmValue("Dimmer")]public SlmToggleValue<MinMaxEasingValue> lightToggleDimmer;// = new StageLightProperty<float>(){value = 1f};
        public LightIntensityProperty()
        {
            propertyOverride = true;
            propertyName = "Dimmer";
            clockOverride = new SlmToggleValue<ClockOverride>();
            lightToggleDimmer = new SlmToggleValue<MinMaxEasingValue>(){value = new MinMaxEasingValue()
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
            lightToggleDimmer.propertyOverride = toggle;
        }
        
        public LightIntensityProperty( LightIntensityProperty other )
        {
            propertyName = other.propertyName;
            propertyOverride = other.propertyOverride;
            clockOverride = new SlmToggleValue<ClockOverride>(other.clockOverride);
            lightToggleDimmer = new SlmToggleValue<MinMaxEasingValue>()
            {
                propertyOverride = other.lightToggleDimmer.propertyOverride,
                value = new MinMaxEasingValue(other.lightToggleDimmer.value)
            };
        }

        public override void OverwriteProperty(SlmProperty other)
        {
            if (other is LightIntensityProperty)
            {
                var otherProperty = other as LightIntensityProperty;
                if (other.propertyOverride)
                {
                    if(otherProperty.lightToggleDimmer.propertyOverride) lightToggleDimmer.value = new MinMaxEasingValue(otherProperty.lightToggleDimmer.value);
                    if(otherProperty.clockOverride.propertyOverride) clockOverride = new SlmToggleValue<ClockOverride>(otherProperty.clockOverride);
                }
                    
            }
        }
    }
}