using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using DarkNaku.Foundation.Extension;

namespace DarkNaku.Attribute
{
    [CustomPropertyDrawer(typeof(AutomaticReferenceAttribute), true)]
    public class AutomaticReferenceDrawer : PropertyDrawer
    {
        private AutomaticReferenceAttribute Attribute => attribute as AutomaticReferenceAttribute;

        private GUIStyle ErrorStyle
        {
            get
            {
                if (_errorStyle == null)
                {
                    _errorStyle = new GUIStyle(EditorStyles.label);
                    _errorStyle.normal.textColor = Color.red;
                }

                return _errorStyle;
            }
        }

        private GUIStyle _errorStyle;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (EditorApplication.isPlaying)
            {
                EditorGUI.PropertyField(position, property, label, true);
                return;
            }

            var fieldType = fieldInfo.FieldType;
            
            if (fieldType.IsSubclassOf(typeof(Component)) == false)
            {
                EditorGUI.LabelField(position,
                    $"{fieldInfo.Name} : Component를 상속받은 타입에만 사용할 수 있습니다.",
                    ErrorStyle);
                return;
            }

            if (property.objectReferenceValue == null ||
                property.objectReferenceValue.GetType().IsSubclassOf(fieldType) == false)
            {
                Component foundComponent = null;
                var fieldName = fieldInfo.Name.Replace("_", "").ToLower();
                var target = property.serializedObject.targetObject as Component;

                switch (Attribute.ReferenceType)
                {
                    case REFERENCE_TYPE.InChildren:
                    {
                        target.transform.ForEachChild((child) =>
                        {
                            var component = child.GetComponent(fieldType);

                            if (component == null) return true;

                            if (Attribute.UseSameName && child.name.ToLower().Equals(fieldName) == false)
                            {
                                return true;
                            }

                            foundComponent = component;

                            return false;
                        }, true);
                        break;
                    }
                    case REFERENCE_TYPE.InParent:
                    {
                        if (target.transform.parent != null)
                        {
                            var components = target.transform.parent.GetComponentsInParent(fieldType, true);

                            for (int i = 0; i < components.Length; i++)
                            {
                                if (Attribute.UseSameName && components[i].name.ToLower().Equals(fieldName) == false)
                                {
                                    continue;
                                }

                                foundComponent = components[i];
                                break;
                            }
                        }

                        break;
                    }
                    case REFERENCE_TYPE.Find:
                    {
                        var components = GameObject.FindObjectsOfType(fieldType, true) as Component[];

                        for (int i = 0; i < components.Length; i++)
                        {
                            if (Attribute.UseSameName && components[i].name.ToLower().Equals(fieldName) == false)
                            {
                                continue;
                            }

                            foundComponent = components[i];
                            break;
                        }

                        break;
                    }
                    default:
                        foundComponent = target.GetComponent(fieldType);
                        break;
                }

                property.objectReferenceValue = foundComponent;

                EditorGUI.PropertyField(position, property, label, true);
            }
        }
    }
}