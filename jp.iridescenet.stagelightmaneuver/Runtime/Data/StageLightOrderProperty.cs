using System;
using System.Collections.Generic;
using UnityEngine;

namespace StageLightManeuver
{
    [Serializable]
    public class StageLightOrderQueue
    {
        public int index = -1;
        public List<StageLightOrderSetting> stageLightOrderSettingList = new List<StageLightOrderSetting>();
        
        public int GetStageLightIndex(LightFixture lightFixture)
        {
            
            if(lightFixture == null) return 0;
           
            if (index < 0)
            {
                return lightFixture.order;
            }
           
            if(stageLightOrderSettingList.Count > index)
            {
                var stageLightOrderSetting = stageLightOrderSettingList[index];
                foreach (var stageLightData in stageLightOrderSetting.stageLightOrder)
                {
                    if (stageLightData.lightFixture == lightFixture)
                    {
                        return stageLightData.index;
                    }
                }
            }
           
            return lightFixture.order;
        }
    }
    [SlmProperty(isRemovable: false)]
    public class StageLightOrderProperty:SlmProperty
    {
        public StageLightOrderQueue stageLightOrderQueue = new StageLightOrderQueue();
        
        
        public StageLightOrderProperty ()
        {
            
            propertyOrder = -998;
            propertyName = "LightFixture Order";
            propertyOverride = true;
            stageLightOrderQueue.stageLightOrderSettingList = new List<StageLightOrderSetting>();
            
        }

        public override void InitStageLightSupervisor(StageLightSupervisor stageLightSupervisor)
        {
            stageLightOrderQueue.stageLightOrderSettingList = stageLightSupervisor.stageLightOrderSettings;
        }

        public StageLightOrderProperty( StageLightOrderProperty other)
        {
            propertyOrder = -998;
            propertyName = other.propertyName;
            propertyOverride = other.propertyOverride;
            stageLightOrderQueue = new StageLightOrderQueue() { index = other.stageLightOrderQueue.index };
            // lightOrder = new SlmToggleValue<int>() { value = other.lightOrder.value };
            // stageLightOrderSetting = new SlmToggleValue<List<StageLightIndex>>() { value = other.stageLightOrderSetting.value };
        }

        public override void ToggleOverride(bool toggle)
        {
            base.ToggleOverride(toggle);
            propertyOverride = toggle;
            // lightOrder.propertyOverride = toggle;
            // stageLightOrderSetting.propertyOverride = toggle;
            
        }

        public override void OverwriteProperty(SlmProperty other)
        {
            base.OverwriteProperty(other);
            StageLightOrderProperty stageLightOrderProperty = other as StageLightOrderProperty;
            if (stageLightOrderProperty == null) return;
            
            // if (stageLightOrderProperty.lightOrder.propertyOverride) lightOrder.value = stageLightOrderProperty.lightOrder.value;
            if (stageLightOrderProperty.propertyOverride)
            {
                // if (stageLightOrderProperty.stageLightOrderSetting.propertyOverride)
                // {
                //     stageLightOrderSetting.value = stageLightOrderProperty.stageLightOrderSetting.value;
                // }
            }


        }
    }
}