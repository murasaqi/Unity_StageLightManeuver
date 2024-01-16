using System;
using System.Collections.Generic;
using UnityEngine;

namespace StageLightManeuver
{
    [Serializable]
    public class StageLightOrderQueue
    {
        public int index = -1;
        public List<StageLightFixtureOrderSetting> stageLightOrderSettingList = new List<StageLightFixtureOrderSetting>();
        
        public int GetStageLightIndex(StageLightFixture stageLightFixture)
        {
            
            if(stageLightFixture == null) return 0;
           
            if (index < 0)
            {
                return stageLightFixture.order;
            }
           
            if(stageLightOrderSettingList.Count > index)
            {
                var stageLightOrderSetting = stageLightOrderSettingList[index];
                foreach (var stageLightData in stageLightOrderSetting.stageLightFixtureOrder)
                {
                    if (stageLightData.stageLightFixture == stageLightFixture)
                    {
                        return stageLightData.index;
                    }
                }
            }
           
            return stageLightFixture.order;
        }
    }
    [SlmProperty(isRemovable: false)]
    public class StageLightOrderProperty:SlmProperty
    {
        public StageLightOrderQueue stageLightOrderQueue = new StageLightOrderQueue();
        
        
        public StageLightOrderProperty ()
        {
            
            propertyOrder = -998;
            propertyName = "StageLightFixture Order";
            propertyOverride = true;
            stageLightOrderQueue.stageLightOrderSettingList = new List<StageLightFixtureOrderSetting>();
            
        }

        public override void InitStageLightFixture(StageLightFixtureBase stageLightFixtureBase)
        {
            if (stageLightFixtureBase is StageLightUniverse stageLightUniverse)
            {
                stageLightOrderQueue.stageLightOrderSettingList = stageLightUniverse.stageLightFixtureOrderSettings;
            }
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