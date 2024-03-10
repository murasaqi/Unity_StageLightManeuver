// <author>
//   douduck08: https://github.com/douduck08
//   Use Reflection to get instance of Unity's SerializedProperty in Custom Editor.
//   Modified codes from 'Unity Answers', in order to apply on nested List<T> or Array. 
//   
//   Original author: HiddenMonk & Johannes Deml
//   Ref: http://answers.unity3d.com/questions/627090/convert-serializedproperty-to-custom-class.html
// </author>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace StageLightManeuver
{
    public static class SerializedPropertyExtensions
    {

        private static int GetArrayIndexFromPropertyName(string propertyNameWithIndex, out string purePropertyName)
        {
            if (!propertyNameWithIndex.EndsWith("]"))
                throw new ArgumentException();
            
            int indexStart = propertyNameWithIndex.IndexOf("[", StringComparison.Ordinal) + 1;

            if (!int.TryParse(propertyNameWithIndex.AsSpan()[indexStart..^1], out int index))
                throw new ArgumentException();

            purePropertyName = propertyNameWithIndex[..(indexStart - 1)];

            return index;
        }

        public static T GetValue<T>(this SerializedProperty property) where T : class
        {
            object obj = property.serializedObject.targetObject;
            string path = property.propertyPath.Replace(".Array.data", "");
            string[] fieldStructure = path.Split('.');
            for (int i = 0; i < fieldStructure.Length; i++)
            {
                if (fieldStructure[i].Contains("["))
                {
                    int index = GetArrayIndexFromPropertyName(fieldStructure[i], out var purePropertyName);
                    obj = GetFieldValueWithIndex(purePropertyName, obj, index);
                }
                else
                {
                    obj = GetFieldValue(fieldStructure[i], obj);
                }
            }

            return (T)obj;
        }

        public static bool SetValue<T>(this SerializedProperty property, T value) where T : class
        {
            object obj = property.serializedObject.targetObject;
            string path = property.propertyPath.Replace(".Array.data", "");
            string[] fieldStructure = path.Split('.');
            for (int i = 0; i < fieldStructure.Length - 1; i++)
            {
                if (fieldStructure[i].Contains("["))
                {
                    int index = GetArrayIndexFromPropertyName(fieldStructure[i], out var purePropertyName);
                    obj = GetFieldValueWithIndex(purePropertyName, obj, index);
                }
                else
                {
                    obj = GetFieldValue(fieldStructure[i], obj);
                }
            }

            string fieldName = fieldStructure.Last();
            if (fieldName.Contains("["))
            {
                int index = GetArrayIndexFromPropertyName(fieldName, out var purePropertyName);
                return SetFieldValueWithIndex(purePropertyName, obj, index, value);
            }
            else
            {
                Debug.Log(value);
                return SetFieldValue(fieldName, obj, value);
            }
        }

        private static object GetFieldValue(string fieldName, object obj,
            BindingFlags bindings = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public |
                                    BindingFlags.NonPublic)
        {
            if (obj == null) return null;
            FieldInfo field = obj.GetType().GetField(fieldName, bindings);
            if (field != null)
            {
                return field.GetValue(obj);
            }

            return default(object);
        }

        private static object GetFieldValueWithIndex(string fieldName, object obj, int index,
            BindingFlags bindings = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public |
                                    BindingFlags.NonPublic)
        {
            FieldInfo field = obj.GetType().GetField(fieldName, bindings);
            if (field != null)
            {
                object list = field.GetValue(obj);
                if (list.GetType().IsArray)
                {
                    return ((object[])list)[index];
                }
                else if (list is IEnumerable)
                {
                    var ilist = list as IList;
                    if (ilist.Count < index)
                    {
                        return null;
                    }
                    else
                    {
                        return ((IList)list)[index];
                    }
                }
            }

            return default(object);
        }

        public static bool SetFieldValue(string fieldName, object obj, object value, bool includeAllBases = false,
            BindingFlags bindings = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public |
                                    BindingFlags.NonPublic)
        {
            FieldInfo field = obj.GetType().GetField(fieldName, bindings);
            if (field != null)
            {
                field.SetValue(obj, value);
                return true;
            }

            return false;
        }

        public static bool SetFieldValueWithIndex(string fieldName, object obj, int index, object value,
            bool includeAllBases = false,
            BindingFlags bindings = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public |
                                    BindingFlags.NonPublic)
        {
            FieldInfo field = obj.GetType().GetField(fieldName, bindings);
            if (field != null)
            {
                object list = field.GetValue(obj);
                if (list.GetType().IsArray)
                {
                    ((object[])list)[index] = value;
                    return true;
                }
                else if (value is IEnumerable)
                {
                    ((IList)list)[index] = value;
                    return true;
                }
            }

            return false;
        }
        
        
      
    }
}