using System.Collections;
using System.Collections.Generic;
using Codice.CM.SEIDInfo;
using JetBrains.Annotations;
using UnityEngine;

namespace DarkNaku.Attribute
{
    public class GetComponentBaseAttribute : PropertyAttribute
    {
        public string Name => _name;

        private string _name;

        public GetComponentBaseAttribute(string name)
        {
            _name = name;
        }
    }
    
    public class GetComponentAttribute : GetComponentBaseAttribute
    {
        public GetComponentAttribute(string name = null) : base(name)
        {
        }
    }
    
    public class GetComponentInChildrenAttribute : GetComponentBaseAttribute
    {
        public GetComponentInChildrenAttribute(string name = null) : base(name)
        {
        }
    }
    
    public class GetComponentsInChildrenAttribute : GetComponentBaseAttribute
    {
        public GetComponentsInChildrenAttribute(string name = null) : base(name)
        {
        }
    }
    
    public class GetComponentInParentAttribute : GetComponentBaseAttribute
    {
        public GetComponentInParentAttribute(string name = null) : base(name)
        {
        }
    }
    
    public class FindComponentAttribute : GetComponentBaseAttribute
    {
        public FindComponentAttribute(string name = null) : base(name)
        {
        }
    }
}