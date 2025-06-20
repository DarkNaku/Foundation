using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;

namespace DarkNaku.Foundation {
    public class FSMBehaviour<S, M> : MonoBehaviour where M : FSMBehaviour<S, M> {
        private Dictionary<S, FSMState<S, M>> _states = new Dictionary<S, FSMState<S, M>>();

        public S State { get; private set; }

        public bool IsRunning { get; private set; }

        public void Run(S state) {
            if (IsRunning) return;

            State = state;

            Run();
        }

        public void Run() {
            if (IsRunning) return;

            IsRunning = true;

            _states[State].OnEnter();
        }

        public void Pause() {
            if (IsRunning == false) return;

            IsRunning = false;
        }

        public void Resume() {
            if (IsRunning) return;

            IsRunning = true;
        }

        public void Stop() {
            if (IsRunning == false) return;

            _states[State].OnExit();

            IsRunning = false;
        }

        public void Change(S state) {
            if (EqualityComparer<S>.Default.Equals(state, State)) return;

            _states[State].OnExit();

            var prevState = State;

            State = state;

            _states[State].OnEnter();

            OnTransition(prevState, State);
        }

        protected virtual void OnTransition(S prevState, S nextState) { }
        protected virtual void EventFixedUpdate() { }
        protected virtual void EventUpdate() { }
        protected virtual void EventLateUpdate() { }
        protected virtual void EventOnMouseDown() { }
        protected virtual void EventOnMouseDrag() { }
        protected virtual void EventOnMouseEnter() { }
        protected virtual void EventOnMouseExit() { }
        protected virtual void EventOnMouseOver() { }
        protected virtual void EventOnMouseUp() { }
        protected virtual void EventOnMouseUpAsButton() { }

        protected void AddStates(params FSMState<S, M>[] states) {
            Assert.IsNotNull(states, "[FSM] AddStates : Parameter can be not null.");

            for (int i = 0; i < states.Length; i++) {
                AddState(states[i]);
            }
        }

        protected void RemoveState(S state) => _states.Remove(state);

        private void FixedUpdate() {
            if (IsRunning) {
                _states[State].FixedUpdate();
                EventFixedUpdate();
            }
        }

        private void Update() {
            if (IsRunning) {
                _states[State].Update();
                EventUpdate();
            }
        }

        private void LateUpdate() {
            if (IsRunning) {
                _states[State].LateUpdate();
                EventLateUpdate();
            }
        }

        private void OnMouseDown() {
            if (IsRunning) {
                _states[State].OnMouseDown();
                EventOnMouseDown();
            }
        }

        private void OnMouseDrag() {
            if (IsRunning) {
                _states[State].OnMouseDrag();
                EventOnMouseDrag();
            }
        }

        private void OnMouseEnter() {
            if (IsRunning) {
                _states[State].OnMouseEnter();
                EventOnMouseEnter();
            }
        }

        private void OnMouseExit() {
            if (IsRunning) {
                _states[State].OnMouseExit();
                EventOnMouseExit();
            }
        }

        private void OnMouseOver() {
            if (IsRunning) {
                _states[State].OnMouseOver();
                EventOnMouseOver();
            }
        }

        private void OnMouseUp() {
            if (IsRunning) {
                _states[State].OnMouseUp();
                EventOnMouseUp();
            }
        }

        private void OnMouseUpAsButton() {
            if (IsRunning) {
                _states[State].OnMouseUpAsButton();
                EventOnMouseUpAsButton();
            }
        }

        private void AddState(FSMState<S, M> state) {
            if (state == null) return;
            if (_states.ContainsKey(state.State)) return;

            if (_states.TryAdd(state.State, state)) {
                if (_states.Count == 1) {
                    State = state.State;
                }

                state.Initialize(this as M);
            }
        }
    }
}