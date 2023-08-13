using System;

namespace StageLightManeuver
{
    [Serializable]
    public class ReflectionProbeProperty:SlmAdditionalProperty
    {
        public SlmToggleValue<MinMaxEasingValue> intensity = new SlmToggleValue<MinMaxEasingValue>();
        
        public ReflectionProbeProperty(ReflectionProbeProperty reflectionProbeProperty)
        {
            propertyName = reflectionProbeProperty.propertyName;
            clockOverride = new SlmToggleValue<ClockOverride>()
            {
                propertyOverride = reflectionProbeProperty.clockOverride.propertyOverride,
                value = new ClockOverride(reflectionProbeProperty.clockOverride.value)
            };
            intensity = new SlmToggleValue<MinMaxEasingValue>()
            {
                propertyOverride = reflectionProbeProperty.intensity.propertyOverride,
                value = new MinMaxEasingValue(reflectionProbeProperty.intensity.value)
            };
            propertyOverride = reflectionProbeProperty.propertyOverride;
        }
        
        public ReflectionProbeProperty()
        {
            propertyName = "Reflection Probe";
            propertyOverride = true;
            clockOverride = new SlmToggleValue<ClockOverride>();
            intensity = new SlmToggleValue<MinMaxEasingValue>() {value = new MinMaxEasingValue()};
        }
        
        public override void ToggleOverride(bool toggle)
        {
            base.ToggleOverride(toggle);
            propertyOverride = toggle;
            intensity.propertyOverride = toggle;
            clockOverride.propertyOverride = toggle;
        }
        
        public override void OverwriteProperty(SlmProperty other)
        {
            base.OverwriteProperty(other);
            var reflectionProbeProperty = other as ReflectionProbeProperty;
            intensity.value = new MinMaxEasingValue(reflectionProbeProperty.intensity.value);
            clockOverride.value = new ClockOverride(reflectionProbeProperty.clockOverride.value);
        }
    }
}