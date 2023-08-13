#if USE_VLB_ALTER
using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace StageLightManeuver
{
    [Serializable]
    public class GoboProperty:SlmAdditionalProperty
    {
        [SlmValue("Gobo Texture")]public SlmToggleValue<Texture2D> goboTexture;
        [SlmValue("Gobo Property Name")]public SlmToggleValue<string> goboPropertyName;
        [FormerlySerializedAs("goroRotationSpeed")] [SlmValue("Rotation Speed")]public SlmToggleValue<MinMaxEasingValue> goboRotationSpeed;

        public GoboProperty()
        {
            propertyName = "Gobo";
            propertyOverride = false;
            clockOverride = new SlmToggleValue<ClockOverride>();
            goboTexture = new SlmToggleValue<Texture2D>(){value = null};
            goboPropertyName = new SlmToggleValue<string>(){value = "_GoboTexture"};
            goboRotationSpeed = new SlmToggleValue<MinMaxEasingValue>(){value = new MinMaxEasingValue()};
        }

        public override void ToggleOverride(bool toggle)
        {
            base.ToggleOverride(toggle); 
            propertyOverride = toggle;
            goboTexture.propertyOverride = toggle;
            goboPropertyName.propertyOverride = toggle;
            goboRotationSpeed.propertyOverride = toggle;
        }

        public GoboProperty(GoboProperty other)
        {
            propertyName = other.propertyName;
            propertyOverride = other.propertyOverride;
            clockOverride = new SlmToggleValue<ClockOverride>(other.clockOverride);
            goboTexture = new SlmToggleValue<Texture2D>(){value =  other.goboTexture.value};
            goboPropertyName = new SlmToggleValue<string>(){value = other.goboPropertyName.value};
            goboRotationSpeed = new SlmToggleValue<MinMaxEasingValue>()
            {
                propertyOverride = other.goboRotationSpeed.propertyOverride,
                value = new MinMaxEasingValue( other.goboRotationSpeed.value)
            };
        }
    }
}
#endif