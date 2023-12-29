using UnityEngine;
namespace StageLightManeuver
{
    public class MaterialFloatProperty:SlmAdditionalProperty
    {
        public SlmToggleValue<MinMaxEasingValue> floatValue;

        public MaterialFloatProperty()
        {
            propertyName = "Material Float";
            clockOverride = new SlmToggleValue<ClockOverride>();
            floatValue = new SlmToggleValue<MinMaxEasingValue>()
            {
                value = new MinMaxEasingValue()
                {
                    minMaxLimit = new Vector2(0, 2),
                    mode = AnimationMode.Constant,
                    constant = 1
                }
            };
        }

        public override void ToggleOverride(bool toggle)
        {
            base.ToggleOverride(toggle);
            floatValue.propertyOverride = toggle;
            clockOverride.propertyOverride = toggle;
        }
        
        public MaterialFloatProperty(MaterialFloatProperty materialFloatProperty)
        {
            propertyName = materialFloatProperty.propertyName;
            clockOverride = new SlmToggleValue<ClockOverride>();
            floatValue = new SlmToggleValue<MinMaxEasingValue>()
            {
                propertyOverride = materialFloatProperty.floatValue.propertyOverride,
                value = new MinMaxEasingValue(materialFloatProperty.floatValue.value)
            };
        }
        
        public override void OverwriteProperty(SlmProperty other)
        {
            MaterialFloatProperty materialFloatProperty = other as MaterialFloatProperty;
            if (materialFloatProperty == null) return;
            floatValue.value = new MinMaxEasingValue(materialFloatProperty.floatValue.value);
        }
    }
}