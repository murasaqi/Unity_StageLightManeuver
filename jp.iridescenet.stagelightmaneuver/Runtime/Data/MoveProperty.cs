using System;
using UnityEngine;

namespace StageLightManeuver
{
    [Serializable]
    public class XTransformProperty:SlmAdditionalProperty
    {
        [SlmValue("X Transform")]public SlmToggleValue<MinMaxEasingValue> positionX; 
        public SlmToggleValue<float> smoothTime = new SlmToggleValue<float>(); 
        public SlmToggleValue<bool> useSmoothness = new SlmToggleValue<bool>();

        public XTransformProperty()
        {
            propertyName = "X Transform";
            propertyOverride = true;
            clockOverride = new SlmToggleValue<ClockOverride>();
            positionX = new SlmToggleValue<MinMaxEasingValue>() {value = new MinMaxEasingValue()
            {
                minMaxLimit = new Vector2(-1,1),
                minMaxValue =   new Vector2(-1,1),
                
            }};
            smoothTime = new SlmToggleValue<float>() {value = 0.05f};
            useSmoothness = new SlmToggleValue<bool>() {value = false};
        }
        public XTransformProperty(XTransformProperty xTransformProperty)
        {
            propertyName = xTransformProperty.propertyName;

            clockOverride = new SlmToggleValue<ClockOverride>()
            {
                propertyOverride = xTransformProperty.clockOverride.propertyOverride,
                value = new ClockOverride(xTransformProperty.clockOverride.value)
            };
            this.positionX = new SlmToggleValue<MinMaxEasingValue>()
            {
                propertyOverride =  xTransformProperty.positionX.propertyOverride,
                value =     new MinMaxEasingValue(xTransformProperty.positionX.value),
            };
            smoothTime = new SlmToggleValue<float>()
            {
                propertyOverride = xTransformProperty.smoothTime.propertyOverride,
                value = xTransformProperty.smoothTime.value
            };
            useSmoothness = new SlmToggleValue<bool>()
            {
                propertyOverride = xTransformProperty.useSmoothness.propertyOverride,
                value = xTransformProperty.useSmoothness.value
            };
            propertyOverride = xTransformProperty.propertyOverride;

        }
        
        public override void ToggleOverride(bool toggle)
        {
            base.ToggleOverride(toggle);
            propertyOverride = toggle;
            positionX.propertyOverride = toggle;
            clockOverride.propertyOverride = toggle;
            smoothTime.propertyOverride = toggle;
            useSmoothness.propertyOverride = toggle;
            
        }

        public override void OverwriteProperty(SlmProperty other)
        {
            base.OverwriteProperty(other);
            var xTransformProperty = (XTransformProperty)other;
            if (xTransformProperty == null) return;
            if(xTransformProperty.positionX.propertyOverride) positionX.value = xTransformProperty.positionX.value;
            if(xTransformProperty.clockOverride.propertyOverride) clockOverride.value = xTransformProperty.clockOverride.value;
            if(xTransformProperty.smoothTime.propertyOverride) smoothTime.value = xTransformProperty.smoothTime.value;
            if(xTransformProperty.useSmoothness.propertyOverride) useSmoothness.value = xTransformProperty.useSmoothness.value;
        }
    }


    [Serializable]
    public class YTransformProperty : SlmAdditionalProperty
    {
        [SlmValue("Y Transform")]public SlmToggleValue<MinMaxEasingValue> positionY; 
        public SlmToggleValue<float> smoothTime = new SlmToggleValue<float>(); 
        public SlmToggleValue<bool> useSmoothness = new SlmToggleValue<bool>(); 
        
        public YTransformProperty()
        {
            propertyName = "Y Transform";
            propertyOverride = true;
            clockOverride = new SlmToggleValue<ClockOverride>();
            positionY = new SlmToggleValue<MinMaxEasingValue>() {value = new MinMaxEasingValue()
            {
                minMaxLimit = new Vector2(-1,1),
                minMaxValue =   new Vector2(-1,1),
                
            }};
            smoothTime = new SlmToggleValue<float>() {value = 0.05f};
            useSmoothness = new SlmToggleValue<bool>() {value = false};
        }
        
        public YTransformProperty(YTransformProperty yTransformProperty)
        {
            propertyName = yTransformProperty.propertyName;

            clockOverride = new SlmToggleValue<ClockOverride>()
            {
                propertyOverride = yTransformProperty.clockOverride.propertyOverride,
                value = new ClockOverride(yTransformProperty.clockOverride.value)
            };
            this.positionY = new SlmToggleValue<MinMaxEasingValue>()
            {
                propertyOverride =  yTransformProperty.positionY.propertyOverride,
                value =     new MinMaxEasingValue(yTransformProperty.positionY.value),
            };
            smoothTime = new SlmToggleValue<float>()
            {
                propertyOverride = yTransformProperty.smoothTime.propertyOverride,
                value = yTransformProperty.smoothTime.value
            };
            useSmoothness = new SlmToggleValue<bool>()
            {
                propertyOverride = yTransformProperty.useSmoothness.propertyOverride,
                value = yTransformProperty.useSmoothness.value
            };
            propertyOverride = yTransformProperty.propertyOverride;

        }
        
        public override void ToggleOverride(bool toggle)
        {
            base.ToggleOverride(toggle);
            propertyOverride = toggle;
            positionY.propertyOverride = toggle;
            clockOverride.propertyOverride = toggle;
            smoothTime.propertyOverride = toggle;
            useSmoothness.propertyOverride = toggle;
            
        }

        public override void OverwriteProperty(SlmProperty other)
        {
            base.OverwriteProperty(other);
            var yTransformProperty = (YTransformProperty)other;
            if (yTransformProperty == null) return;
            if(yTransformProperty.positionY.propertyOverride) positionY.value = yTransformProperty.positionY.value;
            if(yTransformProperty.clockOverride.propertyOverride) clockOverride.value = yTransformProperty.clockOverride.value;
            if(yTransformProperty.smoothTime.propertyOverride) smoothTime.value = yTransformProperty.smoothTime.value;
            if(yTransformProperty.useSmoothness.propertyOverride) useSmoothness.value = yTransformProperty.useSmoothness.value;
        }
    }

    [Serializable]
    public class ZTransformProperty : SlmAdditionalProperty
    {
        [SlmValue("Z Transform")]public SlmToggleValue<MinMaxEasingValue> positionZ; 
        public SlmToggleValue<float> smoothTime = new SlmToggleValue<float>(); 
        public SlmToggleValue<bool> useSmoothness = new SlmToggleValue<bool>(); 
        
        public ZTransformProperty()
        {
            propertyName = "Z Transform";
            propertyOverride = true;
            clockOverride = new SlmToggleValue<ClockOverride>();
            positionZ = new SlmToggleValue<MinMaxEasingValue>() {value = new MinMaxEasingValue()
            {
                minMaxLimit = new Vector2(-1,1),
                minMaxValue =   new Vector2(-1,1),
                
            }};
            smoothTime = new SlmToggleValue<float>() {value = 0.05f};
            useSmoothness = new SlmToggleValue<bool>() {value = false};
        }
        
        public ZTransformProperty(ZTransformProperty zTransformProperty)
        {
            propertyName = zTransformProperty.propertyName;

            clockOverride = new SlmToggleValue<ClockOverride>()
            {
                propertyOverride = zTransformProperty.clockOverride.propertyOverride,
                value = new ClockOverride(zTransformProperty.clockOverride.value)
            };
            this.positionZ = new SlmToggleValue<MinMaxEasingValue>()
            {
                propertyOverride =  zTransformProperty.positionZ.propertyOverride,
                value =     new MinMaxEasingValue(zTransformProperty.positionZ.value),
            };
            smoothTime = new SlmToggleValue<float>()
            {
                propertyOverride = zTransformProperty.smoothTime.propertyOverride,
                value = zTransformProperty.smoothTime.value
            };
            useSmoothness = new SlmToggleValue<bool>()
            {
                propertyOverride = zTransformProperty.useSmoothness.propertyOverride,
                value = zTransformProperty.useSmoothness.value
            };
            propertyOverride = zTransformProperty.propertyOverride;

        }
        
        public override void ToggleOverride(bool toggle)
        {
            base.ToggleOverride(toggle);
            propertyOverride = toggle;
            positionZ.propertyOverride = toggle;
            clockOverride.propertyOverride = toggle;
            smoothTime.propertyOverride = toggle;
            useSmoothness.propertyOverride = toggle;
            
        }
        
        public override void OverwriteProperty(SlmProperty other)
        {
            base.OverwriteProperty(other);
            var zTransformProperty = (ZTransformProperty)other;
            if (zTransformProperty == null) return;
            if(zTransformProperty.positionZ.propertyOverride) positionZ.value = zTransformProperty.positionZ.value;
            if(zTransformProperty.clockOverride.propertyOverride) clockOverride.value = zTransformProperty.clockOverride.value;
            if(zTransformProperty.smoothTime.propertyOverride) smoothTime.value = zTransformProperty.smoothTime.value;
            if(zTransformProperty.useSmoothness.propertyOverride) useSmoothness.value = zTransformProperty.useSmoothness.value;
        }
        
    }
}