using System;
using UnityEngine;

namespace StageLightManeuver
{
    [Serializable]
    public class LightColorProperty:SlmAdditionalProperty
    {
        [SlmValue("Color")]public SlmToggleValue<Gradient> lightToggleColor;// = new StageLightProperty<float>(){value = 1f};
        public LightColorProperty()
        {
            propertyOverride = true;
            propertyName = "Light Color";
            clockOverride = new SlmToggleValue<ClockOverride>();
            lightToggleColor = new SlmToggleValue<Gradient>(){value = new Gradient()};
        }
        
        public override void ToggleOverride(bool toggle)
        {
            clockOverride.propertyOverride = toggle;
            lightToggleColor.propertyOverride = toggle;
            propertyOverride = toggle;
        }
        
        public LightColorProperty( LightColorProperty other )
        {
            propertyName = other.propertyName;
            propertyOverride = other.propertyOverride;
            clockOverride = new SlmToggleValue<ClockOverride>(other.clockOverride);
            lightToggleColor = new SlmToggleValue<Gradient>()
            {
                propertyOverride = other.lightToggleColor.propertyOverride,
                value = SlmUtility.CopyGradient(other.lightToggleColor.value)
            };
        }

        public override void OverwriteProperty(SlmProperty other)
        {
            var otherProperty = other as LightColorProperty;
            if (otherProperty == null) return;
            if (other.propertyOverride)
            {
                if(otherProperty.lightToggleColor.propertyOverride) lightToggleColor.value = SlmUtility.CopyGradient(otherProperty.lightToggleColor.value);
                if(otherProperty.clockOverride.propertyOverride) clockOverride= new SlmToggleValue<ClockOverride>(otherProperty.clockOverride);
            }
        }
    }
}