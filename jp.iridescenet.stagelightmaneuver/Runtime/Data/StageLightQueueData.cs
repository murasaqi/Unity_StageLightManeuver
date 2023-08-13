using System;
using System.Collections.Generic;
using UnityEngine;

namespace StageLightManeuver
{
    [Serializable]
    public class StageLightQueueData
    {
        public Dictionary<StageLight,int> stageLightOrder = new Dictionary<StageLight, int>();
        [SerializeReference]public List<SlmProperty> stageLightProperties = new List<SlmProperty>();
        public float weight = 1;
        
        
        public StageLightQueueData(StageLightQueueData stageLightQueueData)
        {
            this.stageLightProperties = stageLightQueueData.stageLightProperties;
            this.weight = stageLightQueueData.weight;
        }
        
        public int GetOrder(StageLight stageLight)
        {
            if (stageLightOrder.ContainsKey(stageLight))
            {
                return stageLightOrder[stageLight];
            }
            else
            {
                stageLightOrder.Add(stageLight, 0);
                return 0;
            }
        }
        public StageLightQueueData()
        {
            stageLightProperties = new List<SlmProperty>();
            stageLightProperties.Clear();
            weight = 1f;
        }
        public T TryGetActiveProperty<T>() where T : SlmProperty
        {
            foreach (var property in stageLightProperties)
            {
                if (property == null)
                {
                    continue;
                }
                if (property.GetType() == typeof(T) && property.propertyOverride)
                {
                    return property as T;
                }
            }
            return null;
        }
        
        public SlmAdditionalProperty TryGetActiveProperty(Type T) 
        {
            foreach (var property in stageLightProperties)
            {
                if(property == null)
                {
                    continue;
                }
                if (property.GetType() ==T && property.propertyOverride)
                {
                    return property as SlmAdditionalProperty;
                }
            }
            return null;
        }
        
        
        public bool TryAddProperty(Type T)
        {
           
            foreach (var property in stageLightProperties)
            {
                if (property == null)
                {
                    continue;
                }
                if (property.GetType() == T)
                {
                    return false;
                }
            }
            
            
            var instance =  Activator.CreateInstance(T, new object[] { }) as SlmAdditionalProperty;
            stageLightProperties.Add(instance);
            return true;
        }

        public SlmProperty TryAddGetProperty(Type T) 
        {
            
            foreach (var property in stageLightProperties)
            {
                if (property == null)
                {
                    continue;
                }
                if (property.GetType() == T)
                {
                    return property as SlmProperty;
                }
            }
            
            
            var instance =  Activator.CreateInstance(T, new object[] { }) as SlmProperty;
            stageLightProperties.Add(instance);

            return instance;
        }
    }
}