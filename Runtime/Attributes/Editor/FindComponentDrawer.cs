using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
    
namespace DarkNaku.Attribute
{
    [CustomPropertyDrawer(typeof(FindComponentAttribute), true)]
    public class FindComponentDrawer : GetComponentBaseDrawer
    {
        protected override void UpdateObjectReferenceValue(SerializedProperty property)
        {
            var fieldType = fieldInfo.FieldType;
            var components = GameObject.FindObjectsOfType(fieldType, true) as Component[];

            for (int i = 0; i < components.Length; i++)
            {
                if (IsEqualFieldName(components[i].name))
                {
                    property.objectReferenceValue = components[i];
                }
            }
        }
    }
}