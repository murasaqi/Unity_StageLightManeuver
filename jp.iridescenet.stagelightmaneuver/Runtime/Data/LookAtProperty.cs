using System;

namespace StageLightManeuver
{
    [Serializable]
    public class LookAtProperty:SlmAdditionalProperty
    {
        public SlmToggleValue<int> lookAtIndex;
        public SlmToggleValue<float> lookAtWeight; // weight から lookAtWeight に名前変更
        public SlmToggleValue<float> speed;
        public LookAtProperty()
        {
            propertyName = "Look At";
            propertyOverride = true;
            clockOverride = new SlmToggleValue<ClockOverride>();
            lookAtWeight = new SlmToggleValue<float>(){value = 1f}; // weight から lookAtWeight に名前変更
            lookAtIndex = new SlmToggleValue<int>(){value = 0};
            speed = new SlmToggleValue<float>(){value = 1f};
        }
        
        public override void ToggleOverride(bool toggle)
        {
            base.ToggleOverride(toggle);
            lookAtIndex.propertyOverride = toggle;
            speed.propertyOverride = toggle;
            lookAtWeight.propertyOverride = toggle; // weight から lookAtWeight に名前変更
        }
        
        public LookAtProperty( LookAtProperty other )
        {
            propertyName = other.propertyName;
            propertyOverride = other.propertyOverride;
            propertyOrder = other.propertyOrder;
            
            // 重みプロパティをコピー
            base.weight = other.weight;
            base.hasExplicitWeight = other.hasExplicitWeight;
            
            clockOverride = new SlmToggleValue<ClockOverride>(other.clockOverride);
            lookAtWeight = new SlmToggleValue<float>(other.lookAtWeight); // weight から lookAtWeight に名前変更
            lookAtIndex = new SlmToggleValue<int>(other.lookAtIndex);
            speed = new SlmToggleValue<float>(other.speed);
        }

        public override void OverwriteProperty(SlmProperty other)
        {
            base.OverwriteProperty(other);
            LookAtProperty lookAtProperty = other as LookAtProperty;
            if (lookAtProperty == null) return;
            if(lookAtProperty.lookAtWeight.propertyOverride) lookAtWeight.value = lookAtProperty.lookAtWeight.value; // weight から lookAtWeight に名前変更
            if(lookAtProperty.lookAtIndex.propertyOverride) lookAtIndex.value = lookAtProperty.lookAtIndex.value;
            if(lookAtProperty.speed.propertyOverride) speed.value = lookAtProperty.speed.value;
            if(lookAtProperty.clockOverride.propertyOverride) clockOverride = new SlmToggleValue<ClockOverride>(lookAtProperty.clockOverride);
            
        }
    }
}