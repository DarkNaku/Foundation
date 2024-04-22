using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace DarkNaku.Attribute
{
    [CustomPropertyDrawer(typeof(GetComponentAttribute), true)]
    public class GetComponentDrawer : GetComponentBaseDrawer
    {
        protected override void UpdateObjectReferenceValue(SerializedProperty property)
        {
            var target = property.serializedObject.targetObject as Component;
            
            property.objectReferenceValue = target.GetComponent(fieldInfo.FieldType);
        }
    }
}