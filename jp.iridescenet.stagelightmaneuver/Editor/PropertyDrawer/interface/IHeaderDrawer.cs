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
    /// Drawerにヘッダーを追加するためのインターフェース
    /// </summary>
    public interface IHeaderDrawer
    {
        /// <summary>
        /// ヘッダーの描画
        /// </summary>
        /// <param name="position">描画する位置</param>
        /// <param name="property">描画するプロパティー</param>
        /// <param name="label">ラベル</param>
        /// <param name="backgroundColor">背景色</param>
        /// <param name="isFoldout">折りたたみ可能か</param>
        /// <returns>折りたたみ状態</returns>
        bool DrawHeader(Rect position, SerializedProperty property, GUIContent label, Color? backgroundColor = null, bool isFoldout = true);
    }
}