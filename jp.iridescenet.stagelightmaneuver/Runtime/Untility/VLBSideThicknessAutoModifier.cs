
#if USE_VLB
using System;
using System.Collections;
using UnityEngine;

namespace VLB
{
    // [HelpURL(Consts.Help.UrlEffectPulse)]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(VolumetricLightBeamAbstractBase))]
    [ExecuteAlways]
    public class VLBSideThicknessAutoModifier : MonoBehaviour
    {
        public new const string ClassName = "VLBSideThicknessModifier";

        private VolumetricLightBeamHD _volumetricLightBeamHd;
        private VolumetricLightBeamSD _volumetricLightBeamSd;
        private Light m_Light;
        public AnimationCurve thicknessCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
     
        private const float fresnelPowMax = 10f;

        void OnEnable()
        {
            _volumetricLightBeamHd = GetComponent<VolumetricLightBeamHD>();
            _volumetricLightBeamSd = GetComponent<VolumetricLightBeamSD>();
            if(_volumetricLightBeamSd == null && _volumetricLightBeamHd == null) return;
            m_Light = _volumetricLightBeamHd != null
                ? _volumetricLightBeamHd.lightSpotAttached
                : _volumetricLightBeamSd.lightSpotAttached;
            // Debug.Assert(m_Beam != null);

        }


        private void LateUpdate()
        {
            // m_Beam.
            if(m_Light == null) return;
            var angleDiff = Mathf.Clamp(Mathf.Max(m_Light.spotAngle - m_Light.innerSpotAngle,0)/ 180f,0f,1f);
            var softness = Mathf.Lerp(0, fresnelPowMax, thicknessCurve.Evaluate(angleDiff));
            if (_volumetricLightBeamHd != null)
            {
                _volumetricLightBeamHd.sideSoftness = softness;
            }
            
            if (_volumetricLightBeamSd != null)
            {
                _volumetricLightBeamSd.fresnelPow = softness;
            }
        }
 
    }
}
#endif