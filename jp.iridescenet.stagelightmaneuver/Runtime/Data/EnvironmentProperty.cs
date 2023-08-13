using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;

namespace StageLightManeuver
{
    [Serializable]
    public class EnvironmentProperty:SlmAdditionalProperty
    {
        public SlmToggleValue<AmbientMode> ambientMode = new SlmToggleValue<AmbientMode>();
        [FormerlySerializedAs("intensity")] public SlmToggleValue<MinMaxEasingValue> ambientintensity = new SlmToggleValue<MinMaxEasingValue>();
        public SlmToggleValue<Gradient> ambientColor = new SlmToggleValue<Gradient>();
        
        
        public EnvironmentProperty(EnvironmentProperty environmentProperty)
        {
            propertyName = environmentProperty.propertyName;
            clockOverride = new SlmToggleValue<ClockOverride>()
            {
                propertyOverride = environmentProperty.clockOverride.propertyOverride,
                value = new ClockOverride(environmentProperty.clockOverride.value)
            };
            ambientMode = new SlmToggleValue<AmbientMode>()
            {
                propertyOverride = environmentProperty.ambientMode.propertyOverride,
                value = environmentProperty.ambientMode.value
            };
            ambientintensity = new SlmToggleValue<MinMaxEasingValue>()
            {
                propertyOverride = environmentProperty.ambientintensity.propertyOverride,
                value = new MinMaxEasingValue(environmentProperty.ambientintensity.value)
            };
            ambientColor = new SlmToggleValue<Gradient>()
            {
                propertyOverride = environmentProperty.ambientColor.propertyOverride,
                value = environmentProperty.ambientColor.value
            };
            propertyOverride = environmentProperty.propertyOverride;
        }
        
        public EnvironmentProperty()
        {
            propertyName = "Environment";
            propertyOverride = false;
            clockOverride = new SlmToggleValue<ClockOverride>();
            ambientMode = new SlmToggleValue<AmbientMode>() {value = AmbientMode.Flat};
            ambientintensity = new SlmToggleValue<MinMaxEasingValue>() {value = new MinMaxEasingValue()
            {
                minMaxLimit =  new Vector2(0,8),
            }};
            ambientColor = new SlmToggleValue<Gradient>() {value = new Gradient()};
        }
        
        public override void ToggleOverride(bool toggle)
        {
            base.ToggleOverride(toggle);
            propertyOverride = toggle;
            ambientMode.propertyOverride = toggle;
            ambientintensity.propertyOverride = toggle;
            clockOverride.propertyOverride = toggle;
            ambientColor.propertyOverride = toggle;
        }
        
        public override void OverwriteProperty(SlmProperty other)
        {
            base.OverwriteProperty(other);
            var environmentProperty = other as EnvironmentProperty;
            ambientMode.value = environmentProperty.ambientMode.value;
            ambientintensity.value = new MinMaxEasingValue(environmentProperty.ambientintensity.value);
            clockOverride.value = new ClockOverride(environmentProperty.clockOverride.value);
            ambientColor.value = environmentProperty.ambientColor.value;
        }
    }
}