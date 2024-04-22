using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace DarkNaku.Attribute
{
    public abstract class GetComponentsBaseDrawer : PropertyDrawer
    {
        protected GetComponentBaseAttribute Attribute => attribute as GetComponentBaseAttribute;

        protected string FieldName => _fieldName ??= fieldInfo.Name
            .Replace("_", "")
            .Replace(" ", "")
            .ToLower();

        protected GUIStyle ErrorStyle
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
        private string _fieldName;
        private SerializedProperty _array;
        private int _arraySize;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (EditorApplication.isPlaying)
            {
                EditorGUI.PropertyField(position, property, label, true);
                return;
            }

            if (property.propertyType != SerializedPropertyType.ObjectReference)
            {
                EditorGUI.LabelField(position, $"{fieldInfo.Name} : 참조 타입이 아닙니다.", ErrorStyle);
                return;
            }
            
            /*
            var path = property.propertyPath;

            var array = property.serializedObject.FindProperty(path.Substring(0, path.LastIndexOf('.')));

            if (_array != array || array.arraySize != _arraySize)
            {
                _arraySize = _array.arraySize;
                Debug.Log(property.propertyPath);

                property.ClearArray();
            }
            */

            EditorGUI.PropertyField(position, property, label, true);
        }
    }
}