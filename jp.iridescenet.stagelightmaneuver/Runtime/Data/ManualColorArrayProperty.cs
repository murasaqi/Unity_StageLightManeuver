using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace StageLightManeuver
{

    [Serializable]
    public class ColorPrimitiveValue
    {
        public string name;
        public Color color;

        public ColorPrimitiveValue()
        {
            color = Color.white;
            name = "Color";
        }
    }
    
    
    
    [Serializable]
    public class ManualColorArrayProperty:SlmAdditionalProperty
    {
        public SlmToggleValue<List<ColorPrimitiveValue>> colorValues;
        
        
        public ManualColorArrayProperty ()
        {
            propertyName = "Manual Color Array";
            propertyOverride = true;
            colorValues = new SlmToggleValue<List<ColorPrimitiveValue>>() { value = new List<ColorPrimitiveValue>() };
        }

        public override void ToggleOverride(bool toggle)
        {
            base.ToggleOverride(toggle);
            colorValues.propertyOverride = toggle;
            clockOverride.propertyOverride = toggle;
        }

        public override void ResyncArraySize(List<StageLightFixture> stageLights)
        {
            var colorPrimitiveValues = colorValues.value;
            if(colorPrimitiveValues.Count == stageLights.Count) return;

            if (colorPrimitiveValues.Count < stageLights.Count)
            {
                while (colorPrimitiveValues.Count < stageLights.Count)
                {
                    colorPrimitiveValues.Add(new ColorPrimitiveValue());
                }

            }

            if (colorPrimitiveValues.Count > stageLights.Count)
            {
                while (colorPrimitiveValues.Count > stageLights.Count)
                {
                    colorPrimitiveValues.RemoveAt(colorPrimitiveValues.Count - 1);
                }
            }

            for (int j = 0; j < stageLights.Count; j++)
            {
                // if not index is out of range
                if (j < colorPrimitiveValues.Count && j < stageLights.Count)
                {
                    if (colorPrimitiveValues[j] != null && stageLights[j] != null)
                    {
                        colorPrimitiveValues[j].name = stageLights[j].name;
                    }

                }

            }
        }


        public ManualColorArrayProperty (ManualColorArrayProperty other)
        {
            ColorPrimitiveValue[] copy = new ColorPrimitiveValue[other.colorValues.value.Count];
            propertyName = other.propertyName;
            propertyOverride = other.propertyOverride;
            other.colorValues.value.CopyTo(copy);
            colorValues = new SlmToggleValue<List<ColorPrimitiveValue>>() { value = copy.ToList() };
        }

        public override void OverwriteProperty(SlmProperty other)
        {
            if (other is ManualColorArrayProperty)
            {
                ManualColorArrayProperty otherManualColorArrayProperty = (ManualColorArrayProperty) other;
                var copy = new ColorPrimitiveValue[otherManualColorArrayProperty.colorValues.value.Count];
                otherManualColorArrayProperty.colorValues.value.CopyTo(copy);
                if(otherManualColorArrayProperty.colorValues.propertyOverride)
                    colorValues.value = copy.ToList();
                
                if(otherManualColorArrayProperty.clockOverride.propertyOverride)
                    clockOverride = new SlmToggleValue<ClockOverride>(otherManualColorArrayProperty.clockOverride);
            }              
        }
    }
    
}