
using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace StageLightManeuver
{
    [Serializable]
    public class RotationProperty : SlmAdditionalProperty
    {
        // public SlmToggleValue<Vector3> rotationAxis;
        [SlmValue("Rotation Speed")] public SlmToggleValue<MinMaxEasingValue> rotationSpeed;
        // [SlmValue("test Speed")] public SlmToggleValue<float> v;
        public RotationProperty()
        {
            propertyName = "Rotation";

            propertyOverride = true;
            clockOverride = new SlmToggleValue<ClockOverride>();
            // rotationAxis = new SlmToggleValue<Vector3>(){value = new Vector3(0,0,1)};
            rotationSpeed = new SlmToggleValue<MinMaxEasingValue>()
            {
                value = new MinMaxEasingValue(AnimationMode.Constant, new Vector2(-30,30), new Vector2(-40,40), EaseType.Linear,30, new AnimationCurve(new []{new Keyframe(0,0),new Keyframe(1,40)}))
            };
        }
        
        public override void ToggleOverride(bool toggle)
        {
            propertyOverride = toggle;
            rotationSpeed.propertyOverride=(toggle);
            clockOverride.propertyOverride=(toggle);
            clockOverride.propertyOverride = toggle;
        }

        public override void OverwriteProperty(SlmProperty other)
        {
            if (other.propertyOverride) return;
            var rotationProperty = other as RotationProperty;
            if (rotationProperty == null) return;
            if (rotationProperty.propertyOverride) return;
            if(rotationProperty.rotationSpeed.propertyOverride) rotationSpeed = new SlmToggleValue<MinMaxEasingValue>(rotationProperty.rotationSpeed);
            if(rotationProperty.clockOverride.propertyOverride) clockOverride = new SlmToggleValue<ClockOverride>(rotationProperty.clockOverride);
            
        }

        public RotationProperty(RotationProperty other)
        {
            propertyName = other.propertyName;
            propertyOverride = other.propertyOverride;
            clockOverride = new SlmToggleValue<ClockOverride>(other.clockOverride);
            // rotationAxis = new SlmToggleValue<Vector3>(other.rotationAxis){};
            rotationSpeed = new SlmToggleValue<MinMaxEasingValue>()
            {
                propertyOverride = other.rotationSpeed.propertyOverride,
                value = new MinMaxEasingValue(other.rotationSpeed.value)
            };
        }

    }
}