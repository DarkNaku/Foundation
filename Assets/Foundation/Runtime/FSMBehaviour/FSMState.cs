using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DarkNaku.Foundation {
    public abstract class FSMState<S, M> where M : FSMBehaviour<S, M> {
        public M FSM { get; private set; }
        public abstract S State { get; }
        public virtual void OnInitialize() { }
        public virtual void OnEnter() { }
        public virtual void OnExit() { }
        public virtual void FixedUpdate() { }
        public virtual void Update() { }
        public virtual void LateUpdate() { }
        public virtual void OnMouseDown() { }
        public virtual void OnMouseDrag() { }
        public virtual void OnMouseEnter() { }
        public virtual void OnMouseExit() { }
        public virtual void OnMouseOver() { }
        public virtual void OnMouseUp() { }
        public virtual void OnMouseUpAsButton() { }

        public void Initialize(M machine) {
            FSM = machine;
            OnInitialize();
        }
    }
}