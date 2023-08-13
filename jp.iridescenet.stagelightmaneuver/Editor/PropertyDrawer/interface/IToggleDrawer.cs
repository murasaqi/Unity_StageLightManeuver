using System.Reflection.Emit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace StageLightManeuver
{
    /// <summary>
    /// Toggle付きDrawer用インターフェース
    /// </summary>
    public interface IToggleDrawer
    {
        /// <summary>
        /// プロパティにToggleを追加する
        /// </summary>
        /// <param name="position">描画する位置</param>
        /// <param name="property">描画するプロパティー</param>
        /// <param name="label">ラベル</param>
        /// <returns>トグルの状態</returns>
        bool DrawToggle(Rect position, SerializedProperty property, GUIContent label);
    }
}