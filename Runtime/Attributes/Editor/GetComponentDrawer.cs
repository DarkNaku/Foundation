using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

namespace DarkNaku.Attribute
{
    public abstract class GetComponentBaseDrawer : PropertyDrawer
    {
        protected GetComponentBaseAttribute Attribute => attribute as GetComponentBaseAttribute;

        private string FieldName => _fieldName ??= fieldInfo.Name
            .Replace("_", "")
            .Replace(" ", "")
            .ToLower();

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

        private string RefineName(string name) => 
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
        
        protected Transform FindObjectInChild(Transform root, string name)
        {
            var queue = new Queue<Transform>();
            
            for (var i = 0; i < root.childCount; i++)
            {
                queue.Enqueue(root.GetChild(i));
            }
        
            while (queue.Count > 0)
            {
                var item = queue.Dequeue();

                if (item.name.Equals(name))
                {
                    return item;
                }
                
                for (var i = 0; i < item.childCount; i++)
                {
                    queue.Enqueue(item.GetChild(i));
                }
            }

            return null;
        }
        
        protected List<Component> GetComponentsInChild(Transform root, System.Type type)
        {
            var queue = new Queue<Transform>();
            var components = new List<Component>();
            
            for (var i = 0; i < root.childCount; i++)
            {
                queue.Enqueue(root.GetChild(i));
            }
        
            while (queue.Count > 0)
            {
                var item = queue.Dequeue();

                var component = item.GetComponent(type);
                
                if (component != null)
                {
                    components.Add(component);
                }

                for (var i = 0; i < item.childCount; i++)
                {
                    queue.Enqueue(item.GetChild(i));
                }
            }

            return components;
        }
    }
    
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
            var fieldType = fieldInfo.FieldType;
            var target = property.serializedObject.targetObject as Component;
            var foundComponent = GetComponentsInChild(target.transform, fieldType)
                .FirstOrDefault(component => IsEqualFieldName(component.name));

            property.objectReferenceValue = foundComponent;
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

            foreach (var component in components)
            {
                if (!IsEqualFieldName(component.name)) continue;
                
                property.objectReferenceValue = component;
                break;
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

            foreach (var component in components)
            {
                if (IsEqualFieldName(component.name))
                {
                    property.objectReferenceValue = component;
                }
            }
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

            var array = property.serializedObject.FindProperty(path[..path.LastIndexOf('.')]);

            if (array == null || array.isArray == false) return;
            
            if (array.arraySize != _arraySize)
            {
                UpdateComponents(property, array);
            }

            var index = System.Convert.ToInt32(path[(path.IndexOf('[') + 1)..].Replace("]", ""));

            property.objectReferenceValue = _components[index];
        }

        private void UpdateComponents(SerializedProperty property, SerializedProperty array)
        {
            _components = new List<Component>();
            var fieldType = fieldInfo.FieldType.GetElementType();
            var target = property.serializedObject.targetObject as Component;

            if (Attribute.Name == null)
            {
                _components = GetComponentsInChild(target.transform, fieldType);
            }
            else
            {
                var root = FindObjectInChild(target.transform, Attribute.Name);

                if (root != null)
                {
                    _components = GetComponentsInChild(root, fieldType);
                }
            }

            _arraySize = _components.Count;
            array.arraySize = _arraySize;
        }
    }
    
    [CustomPropertyDrawer(typeof(FindComponentsAttribute), true)]
    public class FindComponentsDrawer : GetComponentBaseDrawer
    {
        private int _arraySize;
        private Component[] _components;

        protected override void UpdateObjectReferenceValue(SerializedProperty property)
        {
            var path = property.propertyPath;

            var array = property.serializedObject.FindProperty(path[..path.LastIndexOf('.')]);

            if (array == null || array.isArray == false) return;

            if (array.arraySize != _arraySize)
            {
                UpdateComponents(property, array);
            }

            var index = System.Convert.ToInt32(path[(path.IndexOf('[') + 1)..].Replace("]", ""));

            property.objectReferenceValue = _components[index];
        }

        private void UpdateComponents(SerializedProperty property, SerializedProperty array)
        {
            var fieldType = fieldInfo.FieldType.GetElementType();

            if (Attribute.Name == null)
            {
                _components = GameObject.FindObjectsOfType(fieldType, true) as Component[];
            }
            else
            {
                var transforms = GameObject.FindObjectsByType<Transform>(FindObjectsInactive.Include, FindObjectsSortMode.None);

                Transform root = transforms.FirstOrDefault(transform => transform.name.Equals(Attribute.Name));

                if (root != null)
                {
                    _components = GetComponentsInChild(root, fieldType).ToArray();
                }
            }

            _arraySize = _components.Length;
            array.arraySize = _arraySize;
        }
    }
}