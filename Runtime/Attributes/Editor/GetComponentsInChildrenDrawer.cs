using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using DarkNaku.Foundation.Extension;

namespace DarkNaku.Attribute
{
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
                UpdateComponents(property, array);
            }

            int index = System.Convert.ToInt32(path.Substring(path.IndexOf('[') + 1).Replace("]", ""));

            property.objectReferenceValue = _components[index];
        }

        private void UpdateComponents(SerializedProperty property, SerializedProperty array)
        {
            _components = new List<Component>();
            var fieldType = fieldInfo.FieldType.GetElementType();
            var target = property.serializedObject.targetObject as Component;

            if (Attribute.Name == null)
            {
                target.transform.ForEachChild((child) =>
                {
                    var component = child.GetComponent(fieldType);

                    if (component != null)
                    {
                        _components.Add(component);
                    }
                    return true;
                }, true);
            
            }
            else
            {
                Transform root = null;

                target.transform.ForEachChild((child) =>
                {
                    if (child.name.Equals(Attribute.Name))
                    {
                        root = child;
                        return false;
                    }

                    return true;
                }, true);

                if (root != null)
                {
                    root.transform.ForEachChild((child) =>
                    {
                        var component = child.GetComponent(fieldType);

                        if (component != null)
                        {
                            _components.Add(component);
                        }
                        return true;
                    }, true);
                }
            }

            _arraySize = _components.Count;
            array.arraySize = _arraySize;
        }
    }
}