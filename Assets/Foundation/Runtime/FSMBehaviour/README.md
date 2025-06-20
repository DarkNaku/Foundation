# FSMBehaviour

MonoBehaviour 베이스에 FSM(Finite State Machine) 프레임워크.

## 목적

- MonoBehaviour 라이프 사이클을 기반으로한 (Finite State Machine) 구현을 위함.

## 사용

1. 상태를 정의할 enum 타입을 선언합니다.

```csharp
public enum CAMERA_STATE { IDLE, MOVE, LOCK }
```

2. FSMBehaviour<S, M>을 상속받는 머신 클래스를 선언합니다.

```csharp
public class CameraController : FSMBehaviour<CAMERA_STATE, CameraController> {
}
```

3. FSMState<S, M>을 상속받는 상태 클래스들을 선언합니다.
```csharp
public class IdleState : FSMState<CAMERA_STATE, CameraController> {
	public override CAMERA_STATE State { get { return CAMERA_STATE.IDLE; } }
}

public class MoveState : FSMState<CAMERA_STATE, CameraController> {
	public override CAMERA_STATE State { get { return CAMERA_STATE.MOVE; } }
}

public class LockState : FSMState<CAMERA_STATE, CameraController> {
	public override CAMERA_STATE State { get { return CAMERA_STATE.LOCK; } }
}
```

4. 머신의 Awake 이벤트에서 상태 객체들을 생성하여 머신에 등록하고 시작함수를 호출합니다.
```csharp
public class CameraController : FSMBehaviour<CAMERA_STATE, CameraController> {
	private void Awake() {
		AddStates(new IdleState(), new MoveState(), new LockState());
        Run(CAMERA_STATE.IDLE);
	}
}
```

## 클래스

### public abstract class FSMBehaviour\<S, M>

머신의 추상 클래스 타입입니다. S는 상태를 나타내는 타입이고 M은 FSMBehaviour를 상속받는 클래스의 타입입니다.

#### 속성

##### public S State { get; }

- 현재 머신의 상태를 나타내는 속성입니다. 읽기 전용.

##### public bool IsRunning { get; }

- 머신을 상태를 나타내는 속성입니다. 기본값은 false 입니다. 읽기 전용

##### public UnityEvent<S, S> OnTransitionEvent { get; }

- 상태 전환 이벤트 등록 속성입니다.



#### 함수

##### public void Change(S state);

- 머신의 상태를 변경하기 위한 함수입니다.

##### protected void AddStates(params FSMState<S, M> states);

- 머신에 상태들을 추가하기 위한 함수입니다.

##### protected void RemoveState(S state);

- 머신에서 상태를 삭제하기 위한 함수입니다.

---

### public abstract class FSMState\<S, M>

- 상태의 추상 클래스입니다. S는 enum 타입, M은 머신 클래스 타입이며, FSMBehaviour<S, M>에서 사용한 타입과 동일한 타입을 선언해야 합니다. 



#### 속성

##### protected M FSM;

- 머신 참조 속성 입니다. 상태 내부에서 머신의 내부의 속성 및 함수를 참조하기 위한 용도 입니다.

##### public abstract S State;

- 상태를 정의하는 속성이며, 상태 클래스를 상속받는 경우 반드시 구현해 줘야 합니다.



#### 함수

##### public virtual void OnInitialize();

- 상태 객체가 머신에 등록될 때 한번 호출 됩니다.

##### public virtual void OnEnter();

- 상태에 진입할 때 한번 호출 됩니다.

##### public virtual void OnExit();

- 상태에서 빠저나올 때 한번 호출 됩니다.

##### public virtual void OnFixedUpdate();

- MonoBehaviour의 FixedUpdate와 동일한 주기로 호출 됩니다.

##### public virtual void OnUpdate();

- MonoBehaviour의 Update와 동일한 주기로 호출 됩니다.

##### public virtual void OnLateUpdate();

- MonoBehaviour의 LateUpdate와 동일한 주기로 호출 됩니다.