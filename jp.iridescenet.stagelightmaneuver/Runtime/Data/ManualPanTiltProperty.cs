using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace StageLightManeuver
{


    [Serializable]
    public class PanTiltPrimitive
    {
        public string name;
        public float pan = 0f;
        public float tilt = 0f;
    }


    [Serializable]
    // ManualPanTiltMode enum Overwrite, Add, Subtract, Multiply, Divide
    public enum ManualPanTiltMode
    {
        Overwrite,
        Add,
        Multiply,
    }
    
    
    [Serializable]
    public class ManualPanTiltProperty:SlmAdditionalProperty
    {
        public SlmToggleValue<ManualPanTiltMode> mode;
        public SlmToggleValue<List<PanTiltPrimitive>> positions;
        
        public ManualPanTiltProperty ()
        {
            propertyName = "Manual Pan Tilt";
            positions = new SlmToggleValue<List<PanTiltPrimitive>>() { value = new List<PanTiltPrimitive>() };
            mode = new SlmToggleValue<ManualPanTiltMode>() { value = ManualPanTiltMode.Overwrite };
            propertyOverride = true;
        }
        
        
        public ManualPanTiltProperty (ManualPanTiltProperty other)
        {
            PanTiltPrimitive[] copy = new PanTiltPrimitive[other.positions.value.Count];
            propertyName = other.propertyName;
            propertyOverride = other.propertyOverride;
            other.positions.value.CopyTo(copy);
            positions = new SlmToggleValue<List<PanTiltPrimitive>>() { value = copy.ToList() };
            mode = new SlmToggleValue<ManualPanTiltMode>() { value = other.mode.value };
            propertyOverride = true;
        }

        public override void ResyncArraySize(List<LightFixture> stageLights)
        {
            base.ResyncArraySize(stageLights);
            var manualPanTiltArray = positions.value;
            if (manualPanTiltArray.Count < stageLights.Count)
            {
                while (manualPanTiltArray.Count < stageLights.Count)
                {
                    manualPanTiltArray.Add(new PanTiltPrimitive());
                }

            }

            if (manualPanTiltArray.Count > stageLights.Count)
            {
                while (manualPanTiltArray.Count > stageLights.Count)
                {
                    manualPanTiltArray.RemoveAt(manualPanTiltArray.Count - 1);
                }
            }

            for (int j = 0; j < stageLights.Count; j++)
            {
                // if not index is out of range
                if (j < manualPanTiltArray.Count && j < stageLights.Count)
                {
                    if (manualPanTiltArray[j] != null && stageLights[j] != null)
                    {
                        manualPanTiltArray[j].name = stageLights[j].name;
                    }

                }

            }
        }

        public override void ToggleOverride(bool toggle)
        {
            base.ToggleOverride(toggle);
            positions.propertyOverride = toggle;
            mode.propertyOverride = toggle;
            clockOverride.propertyOverride = toggle;
            
        }

        public override void OverwriteProperty(SlmProperty other)
        {
            ManualPanTiltProperty manualPanTiltProperty = other as ManualPanTiltProperty;
            if (manualPanTiltProperty == null) return;
            var copy = new PanTiltPrimitive[manualPanTiltProperty.positions.value.Count];
            manualPanTiltProperty.positions.value.CopyTo(copy); 
            if (manualPanTiltProperty.positions.propertyOverride) positions.value = copy.ToList();
            if (manualPanTiltProperty.mode.propertyOverride) mode.value = manualPanTiltProperty.mode.value;
        }
    }
    
    
    
    
}