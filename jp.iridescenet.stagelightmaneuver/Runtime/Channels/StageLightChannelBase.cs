using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.Serialization;

namespace StageLightManeuver
{
    [System.AttributeUsage(System.AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    [BaseTypeRequired(typeof(StageLightChannelBase))]
    public class ChannelFieldAttribute : PropertyAttribute
    {
        public bool IsConfigField;
        private bool _saveToProfile;
        public bool SaveToProfile
        {
            get => _saveToProfile && IsConfigField;
            set => _saveToProfile = value;
        }

        /// <summary>
        /// <para>
        /// <see cref="StageLightChannelBase"/> を継承した、ライトチャンネルのフィールドに使用します。
        /// </para><para>
        /// この属性は、対象のフィールドが灯体の振る舞いを定義するための設定フィールドかどうかを指定します。
        /// インスペクターには設定フィールドに指定されたフィールドのみ表示されます。
        /// </para><para>
        /// フィールドを灯体設定に指定した場合、<paramref name="saveToProfile"/>で<see cref="StageLightFixtureProfile"/>に保存するかどうかを指定できます。
        /// デフォルトでは<c>true</c>です。
        /// ただし、Profileに保存できるのはプリミティブ型のみです。参照型は保存できません。
        /// </para>
        /// <code>
        /// public class SampleChannel : StageLightChannelBase
        /// {
        ///     [ChannelField(true)]            public float fieldA;  //  インスペクタに表示されます。    プロファイルに保存されます。
        ///     [ChannelField(true, false)]     public float fieldB;  //  インスペクタに表示されます。    プロファイルに保存されません。
        ///     [ChannelField(false)]           public float fieldC;  //  インスペクタに表示されません。  プロファイルに保存されません。
        /// }
        /// </code>
        /// </summary>
        /// <param name="isConfigField">設定フィールドかどうか</param>
        /// <param name="saveToProfile">Profileに保存するかどうか</param>
        public ChannelFieldAttribute(bool isConfigField, bool saveToProfile=true)
        {
            IsConfigField = isConfigField;
            SaveToProfile = saveToProfile;
        }
    }

    /// <summary>
    /// <para><see cref="StageLightChannelBase"/> を継承したクラスで使用できます。</para>
    /// フィールドにこの属性を指定すると、インスペクターにフィールドを表示する際に、水平線を表示します。
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    [BaseTypeRequired(typeof(StageLightChannelBase))]
    public class HrAttribute : PropertyAttribute {}

    [Serializable]
    [AddComponentMenu("")]
    [RequireComponent(typeof(StageLightFixture))]
    public abstract class StageLightChannelBase: MonoBehaviour,IStageLightChannel
    {
        public List<LightFixtureBase> SyncStageLight { get; set; }

#region DoNotSaveToProfile-Configs
#endregion


#region Configs
#endregion


#region params
        [ChannelField(false)] public List<Type> PropertyTypes = new List<Type>();
        [ChannelField(false)] public Queue<StageLightQueueData> stageLightDataQueue = new Queue<StageLightQueueData>();
        [ChannelField(false)] public int updateOrder = 0;
        // public int Index { get; set; }
        [ChannelField(false)] public float offsetDuration = 0f;
        // [ChannelFieldBehavior(true, false)] public StageLightFixture ParentStageLight { get; set; }
        [ChannelField(false)] [FormerlySerializedAs("parentStageLight")] public StageLightFixture parentStageLightFixture;
        [ChannelField(false)]internal bool hasQue = false; //! do not use
#endregion


        public virtual void EvaluateQue(float currentTime)
        {

        }

        public virtual void UpdateChannel()
        {
            
        }

        public virtual void Init()
        {
            PropertyTypes.Clear();
            SyncStageLight = new List<LightFixtureBase>();
            foreach (var stageLightChannelBase in GetComponentsInChildren<LightFixtureBase>())
            {
                SyncStageLight.Add(stageLightChannelBase);
            }
        }
    }
}
