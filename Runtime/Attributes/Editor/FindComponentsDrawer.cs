using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using DarkNaku.Foundation.Extension;
    
namespace DarkNaku.Attribute
{
    [CustomPropertyDrawer(typeof(FindComponentsAttribute), true)]
    public class FindComponentsDrawer : GetComponentBaseDrawer
    {
        private int _arraySize;
        private Component[] _components;

        protected override void UpdateObjectReferenceValue(SerializedProperty property)
        {
            var path = property.propertyPath;

            var array = property.serializedObject.FindProperty(path.Substring(0, path.LastIndexOf('.')));

            if (array == null || array.isArray == false) return;

            var fieldType = fieldInfo.FieldType;
            
            if (array.arraySize != _arraySize)
            {
                UpdateComponents(property, array);
            }

            int index = System.Convert.ToInt32(path.Substring(path.IndexOf('[') + 1).Replace("]", ""));

            property.objectReferenceValue = _components[index];
        }

        private void UpdateComponents(SerializedProperty property, SerializedProperty array)
        {
            var fieldType = fieldInfo.FieldType.GetElementType();
            var target = property.serializedObject.targetObject as Component;

            if (Attribute.Name == null)
            {
                _components = GameObject.FindObjectsOfType(fieldType, true) as Component[];
            }
            else
            {
                Transform root = null;

                var transforms = GameObject.FindObjectsByType<Transform>(FindObjectsInactive.Include, FindObjectsSortMode.None);

                foreach (var transform in transforms)
                {
                    if (transform.name.Equals(Attribute.Name))
                    {
                        root = transform;
                        break;
                    }
                }

                if (root != null)
                {
                    var components = new List<Component>();

                    root.transform.ForEachChild((child) =>
                    {
                        var component = child.GetComponent(fieldType);

                        if (component != null)
                        {
                            components.Add(component);
                        }
                        return true;
                    }, true);

                    _components = components.ToArray();
                }
            }

            _arraySize = _components.Length;
            array.arraySize = _arraySize;
        }
    }
}