using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DarkNaku.Foundation;

public class TestFSM : FSMBehaviour<string, TestFSM> {
    private void Awake() {
        AddStates(new IdleState(), new ChaseState());
        StartFSM("Idle");
    }

    public class IdleState : FSMState<string, TestFSM> {
        public override string State => "Idle";

        private float _enterTime;

        public override void OnEnter() {
            Debug.Log("Entered Idle State");

            _enterTime = Time.time;
        }

        public override void Update() {
            if (Time.time - _enterTime > 3f) {
                FSM.ChangeState("Chase");
                return;
            }
        }

        public override void OnExit() {
            Debug.Log("Exited Idle State");
        }
    }

    public class ChaseState : FSMState<string, TestFSM> {
        public override string State => "Chase";

        private float _enterTime;

        public override void OnEnter() {
            Debug.Log("Entered Chase State");

            _enterTime = Time.time;
        }

        public override void Update() {
            if (Time.time - _enterTime > 3f) {
                FSM.ChangeState("Idle");
                return;
            }
        }

        public override void OnExit() {
            Debug.Log("Exited Chase State");
        }
    }
}