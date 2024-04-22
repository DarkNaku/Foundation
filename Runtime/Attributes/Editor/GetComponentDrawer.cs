using System.Collections;
using System.Collections.Generic;
using DarkNaku.Foundation.Extension;
using UnityEditor;
using UnityEngine;

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

                if (IsEqualFieldName(RefineName(component.name)))
                {
                    foundComponent = component;
                    return false;
                }
                
                return true;
            }, true);

            property.objectReferenceValue = foundComponent;
        }
    }
    
    
    [CustomPropertyDrawer(typeof(GetComponentsInChildrenAttribute), true)]
    public class GetComponentsInChildrenDrawer : GetComponentBaseDrawer
    {
        private int _arraySize;
        private List<Component> _components;
        
        protected override void UpdateObjectReferenceValue(SerializedProperty property)
        {
            var path = property.propertyPath;

            var array = property.serializedObject.FindProperty(path.Substring(0, path.LastIndexOf('.')));

            if (array == null || array.isArray == false) return;
            
            if (array.arraySize != _arraySize)
            {
                _components = new List<Component>();
                var fieldType = fieldInfo.FieldType.GetElementType();
                var target = property.serializedObject.targetObject as Component;

                target.transform.ForEachChild((child) =>
                {
                    var component = child.GetComponent(fieldType);

                    if (component != null)
                    {
                        _components.Add(component);
                    }
                    return true;
                }, true);
            
                _arraySize = _components.Count;
                array.arraySize = _arraySize;
            }
            
            int index = System.Convert.ToInt32(path.Substring(path.IndexOf('[') + 1).Replace("]", ""));

            property.objectReferenceValue = _components[index];
        }
    }
    
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
                if (IsEqualFieldName(RefineName(components[i].name)))
                {
                    property.objectReferenceValue = components[i];
                    break;
                }
            }
        }
    }
    
    [CustomPropertyDrawer(typeof(FindComponentAttribute), true)]
    public class FindComponentDrawer : GetComponentBaseDrawer
    {
        protected override void UpdateObjectReferenceValue(SerializedProperty property)
        {
            var fieldType = fieldInfo.FieldType;
            var components = GameObject.FindObjectsOfType(fieldType, true) as Component[];

            for (int i = 0; i < components.Length; i++)
            {
                if (IsEqualFieldName(RefineName(components[i].name)))
                {
                    property.objectReferenceValue = components[i];
                }
            }
        }
    }
}