using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace DarkNaku.Attribute
{
    public abstract class GetComponentBaseDrawer : PropertyDrawer
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
            
            UpdateObjectReferenceValue(property);
            
            EditorGUI.PropertyField(position, property, label, true);
        }

        protected abstract void UpdateObjectReferenceValue(SerializedProperty property);

        protected string RefineName(string name) => 
            name.Replace("_", "")
                .Replace(" ", "")
                .ToLower();

        protected bool IsEqualFieldName(string goName)
        {
            if (Attribute.Name == null)
            {
                if (RefineName(goName).Equals(FieldName)) return true;
            }
            else
            {
                if (goName.Equals(Attribute.Name)) return true;
            }

            return false;
        }
    }
}