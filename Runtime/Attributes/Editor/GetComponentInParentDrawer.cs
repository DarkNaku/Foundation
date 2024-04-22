using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace DarkNaku.Attribute
{
    [CustomPropertyDrawer(typeof(GetComponentInParentAttribute), true)]
    public class GetComponentInParentDrawer : GetComponentBaseDrawer
    {
        protected override void UpdateObjectReferenceValue(SerializedProperty property)
        {
            var target = property.serializedObject.targetObject as Component;
            var parent = target.transform.parent;
            
            if (parent == null) return;
            
            var fieldType = fieldInfo.FieldType;
            var components = parent.GetComponentsInParent(fieldType, true);

            for (int i = 0; i < components.Length; i++)
            {
                if (IsEqualFieldName(components[i].name))
                {
                    property.objectReferenceValue = components[i];
                    break;
                }
            }
        }
    }
}