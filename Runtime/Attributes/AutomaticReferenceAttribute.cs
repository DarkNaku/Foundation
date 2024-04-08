using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DarkNaku.Attribute
{
    public enum REFERENCE_TYPE { Self, InChildren, InParent, Find }

    public class AutomaticReferenceAttribute : PropertyAttribute
    {
        public REFERENCE_TYPE ReferenceType { get; }
        public bool UseSameName { get; }
    
        public AutomaticReferenceAttribute(REFERENCE_TYPE referenceType = REFERENCE_TYPE.Self, bool useSameName = true)
        {
            ReferenceType = referenceType;
            UseSameName = useSameName;
        }
    }
}