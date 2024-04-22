using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using DarkNaku.Foundation.Extension;

namespace DarkNaku.Attribute
{
    [CustomPropertyDrawer(typeof(GetComponentInChildrenAttribute), true)]
    public class GetComponentInChildrenDrawer : GetComponentBaseDrawer
    {
        protected override void UpdateObjectReferenceValue(SerializedProperty property)
        {
            Component foundComponent = null;
            var fieldType = fieldInfo.FieldType;
            var target = property.serializedObject.targetObject as Component;
            
            target.transform.ForEachChild((child) =>
            {
                var component = child.GetComponent(fieldType);

                if (component == null) return true;

                if (IsEqualFieldName(component.name))
                {
                    foundComponent = component;
                    return false;
                }
                
                return true;
            }, true);

            property.objectReferenceValue = foundComponent;
        }
    }
}