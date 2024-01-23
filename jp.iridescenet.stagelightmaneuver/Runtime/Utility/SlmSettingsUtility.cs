using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace StageLightManeuver
{
    public static class SlmSettingsUtility
    {
        /// <summary>
        ///  <see cref="StageLightProfile"/>のデフォルトのエクスポート先のパス
        /// </summary>
        public const string BaseExportProfilePath = "Assets/StageLightManeuver/Profiles/<Scene>/<ClipName>";

        /// <summary>
        /// 全ての<see cref="SlmProperty"/>を含むリストを返す。このリストの順番が<see cref="StageLightProfile"/>のデフォルトの順番になる。
        /// </summary>
        public static readonly List<Type> SlmPropertys = new List<Type>
        {
            typeof(ClockProperty),
            typeof(StageLightOrderProperty),
            typeof(LightProperty),
            typeof(LightIntensityProperty),
            typeof(SlmBarLightProperty),
            typeof(BarLightIntensityProperty),
            typeof(LightColorProperty),
            typeof(LightArrayProperty),
            typeof(ManualLightArrayProperty),
            typeof(ManualColorArrayProperty),
            typeof(MaterialColorProperty),
            typeof(MaterialTextureProperty),
            typeof(SyncLightMaterialProperty),
            typeof(DecalProperty),
#if USE_VLB_ALTER
            typrof(GoboProperty),
#endif
            typeof(XTransformProperty),
            typeof(YTransformProperty),
            typeof(ZTransformProperty),
            typeof(RotationProperty),
            typeof(PanProperty),
            typeof(TiltProperty),
            typeof(BarLightPanProperty),
            typeof(ManualPanTiltProperty),
            typeof(LookAtProperty),
            typeof(SmoothLookAtProperty),
            typeof(EnvironmentProperty),
            typeof(ReflectionProbeProperty),
            typeof(SlmAdditionalProperty),
        };
    }
}