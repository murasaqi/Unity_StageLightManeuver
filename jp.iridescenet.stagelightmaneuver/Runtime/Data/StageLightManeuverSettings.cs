using System.Text;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace StageLightManeuver
{
    [CreateAssetMenu(fileName = "StageLightManeuverSetting", menuName = "StageLightManeuver/GlobalSettings")]
    public class StageLightManeuverSettings : ScriptableObject
    {
        [Tooltip("Default export path for StageLightProfile")]
        public string exportProfilePath = SlmSettingsUtility.BaseExportProfilePath;
        [Tooltip("Default export path for LightFixtureProfile")]
        public string lightFixtureProfileExportPath = SlmSettingsUtility.FixtureProfileDefaultExportPath;
        [SerializeField] 
        private SerializableKeyPair<string,int>[] propertyOrders;
        private Dictionary<string, int> _slmPropertyOrder;
        [Tooltip("Property order for inspector view of StageLightProfile and StageLightClip")]
        public Dictionary<string, int> SlmPropertyOrder 
        {
            get
            {
                if (_slmPropertyOrder == null)
                {
                    _slmPropertyOrder = propertyOrders
                        .ToDictionary(x => x.Key, x => x.Value);
                }
                else
                {
                    SaveSlmPropertyOrder();
                }
                return _slmPropertyOrder;
            }
            set 
            {
                _slmPropertyOrder = value;
                propertyOrders = _slmPropertyOrder
                    .Select(x => new SerializableKeyPair<string, int> { Key = x.Key, Value = x.Value })
                    .ToArray();
            }
        }
        // public GUIStyle[] SlmPropertyStyles;

        public void OnEnable()
        {
            if (propertyOrders == null)
            {
                SlmPropertyOrder = SlmSettingsUtility.SlmPropertys
                    .Select((x, i) => new { x, i })
                    .ToDictionary(x => x.x.Name, x => x.i - 2);
                SlmPropertyOrder[nameof(ClockProperty)] = -999;
                SlmPropertyOrder[nameof(StageLightOrderProperty)] = -998;
                SaveSlmPropertyOrder();
            }
        }
        

        public StageLightManeuverSettings Clone()
        {
            var clone = CreateInstance<StageLightManeuverSettings>();
            clone.exportProfilePath = exportProfilePath;
            clone.SlmPropertyOrder = SlmPropertyOrder;
            return clone;
        }
        
        /// <summary>
        /// <see cref="SlmPropertyOrder"/>の変更を<see cref="propertyOrders"/>に保存する
        /// </summary>
        public void SaveSlmPropertyOrder()
        {
            propertyOrders = _slmPropertyOrder
                .Select(x => new SerializableKeyPair<string, int> { Key = x.Key, Value = x.Value })
                .ToArray();
        }
    }
    
    [Serializable]
    public class SerializableKeyPair<TKey, TValue>
    {
        [SerializeField] private TKey key;
        [SerializeField] private TValue value;

        public TKey Key { get => key; set => key = value; }
        public TValue Value { get => value; set => this.value = value; }
    }
}
