using System;
using UnityEngine;

namespace StageLightManeuver
{
    [Serializable]
    public class LightArrayProperty: SlmAdditionalProperty
    {
        [SlmValue("Spot Angle")]public SlmToggleValue<MinMaxEasingValue> spotAngle;// = new StageLightProperty<float>(){value = 15f};
        [SlmValue("Inner Spot Angle")]public SlmToggleValue<MinMaxEasingValue> innerSpotAngle;// = new StageLightProperty<float>(){value = 10f};
        [SlmValue("Range")]public SlmToggleValue<MinMaxEasingValue> range;
        [SlmValue("Cookie")]public SlmToggleValue<Texture> cookie;
        public LightArrayProperty()
        {
            propertyName = "Light";
            propertyOverride = false;
            clockOverride = new SlmToggleValue<ClockOverride>();
            spotAngle = new SlmToggleValue<MinMaxEasingValue>(){value = new MinMaxEasingValue()
            {
                minMaxValue =  new Vector2(0,180),
                constant = 30f,
                mode = AnimationMode.Constant
            }};
            innerSpotAngle = new SlmToggleValue<MinMaxEasingValue>(){value = new MinMaxEasingValue()
            {
                minMaxValue =  new Vector2(0,180),
                constant = 10f,
                mode = AnimationMode.Constant
            }};
            range =  new SlmToggleValue<MinMaxEasingValue>(){value = new MinMaxEasingValue()
            {
                minMaxValue =  new Vector2(0,100),
                constant = 5f,
                mode = AnimationMode.Constant
            }};
            cookie = new SlmToggleValue<Texture>(){value = null};
        }

        public override void ToggleOverride(bool toggle)
        {
            base.ToggleOverride(toggle);
            propertyOverride = toggle;
            spotAngle.propertyOverride = toggle;
            innerSpotAngle.propertyOverride = toggle;
            range.propertyOverride = toggle;
            cookie.propertyOverride = toggle;
            clockOverride.propertyOverride = toggle;
            
        }

        public LightArrayProperty(LightProperty other)
        {
            propertyName = other.propertyName;
            propertyOverride = other.propertyOverride;
            
            clockOverride = new SlmToggleValue<ClockOverride> (other.clockOverride);
            spotAngle = new SlmToggleValue<MinMaxEasingValue>()
            {
                propertyOverride = other.spotAngle.propertyOverride,
                value = new MinMaxEasingValue(other.spotAngle.value)
            };
            innerSpotAngle = new SlmToggleValue<MinMaxEasingValue>()
            {
                propertyOverride = other.innerSpotAngle.propertyOverride,
                value = new MinMaxEasingValue(other.innerSpotAngle.value)
            };
            range = new SlmToggleValue<MinMaxEasingValue>()
            {
                propertyOverride = other.range.propertyOverride,
                value = new MinMaxEasingValue(other.range.value)
            };
            cookie = new SlmToggleValue<Texture>(other.cookie);
        }

        public override void OverwriteProperty(SlmProperty other)
        {
            base.OverwriteProperty(other);
            LightProperty lightProperty = other as LightProperty;
            if (lightProperty == null) return;
            if(lightProperty.innerSpotAngle.propertyOverride) innerSpotAngle = new SlmToggleValue<MinMaxEasingValue>()
            {
                propertyOverride = lightProperty.innerSpotAngle.propertyOverride,
                value = new MinMaxEasingValue(lightProperty.innerSpotAngle.value)
            };
            if(lightProperty.spotAngle.propertyOverride) spotAngle = new SlmToggleValue<MinMaxEasingValue>()
            {
                propertyOverride = lightProperty.spotAngle.propertyOverride,
                value = new MinMaxEasingValue(lightProperty.spotAngle.value)
            };
            if(lightProperty.range.propertyOverride) range = new SlmToggleValue<MinMaxEasingValue>()
            {
                propertyOverride = lightProperty.range.propertyOverride,
                value = new MinMaxEasingValue(lightProperty.range.value)
            };
            if(lightProperty.cookie.propertyOverride) cookie = new SlmToggleValue<Texture>()
            {
                propertyOverride = lightProperty.cookie.propertyOverride,
                value = lightProperty.cookie.value
            };
            
        }
    }
}