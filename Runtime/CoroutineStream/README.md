# CoroutineStream

코루틴(Coroutine) 순차실행(Sequence Workflow) 라이브러리



## 목적

- 순차(Sequence) 또는 동시(Parallel)에 실행되는 코루틴(Coroutine)들의 코드 복잡도를 줄이고 가독성을 높이기 위함



## 사용

```csharp
    ...
        CSPlayer.CoroutineStream()
            .Append(CoroutineA())
            .Callback(() => { Debug.Log(string.Format("{0} : Hello World !", Time.time)); })
            .Append(CoroutineB(), CoroutineC())
            .Interval(2f)
            .Append(CoroutineD(), CoroutineE(), CoroutineF())
            .Until(() => _flag)
            .LogFormat("{0} : Z Key Down.", Time.time)
            .While(() => Input.GetKey(KeyCode.Z))
            .LogFormat("{0} : Z Key Up.", Time.time)
            .OnComplete(() => { Debug.Log(string.Format("{0} : Completed !!!", Time.time)); });
    ...
```



## 추가 하고 싶은 내용

- 지금 콜백이나 로그가 한 프레임을 먹고 있는데 그 부분을 프레임 손실 없이 실행 할 방법을 고민 중 입니다.



## 문서

### class CSPlayer

- 게임 오브젝트를 갖고 있는 싱글턴 객체로 코루틴을 실행하는 역활하는 클래스.



#### 함수

##### public static CoroutineStream();

- 작업 목록을 등록하고 실행할 객체를 얻어옵니다.

```csharp
  var cs = CSPlayer.CoroutineStream();
```



### class CoroutineStream

- 작업 목록을 관리하는 역활을 하는 클래스.



#### 속성

##### public bool Stoped;

* 작업이 중지 완료 되지 못하고 도중에 중지 된 경우 참이 됩니다.

  

##### public bool Paused;

* 작업이 일시 정지된 경우 참이 됩니다.

  

##### public bool Completed;

* 작업 목록이 모두 완료된 경우 참이 됩니다.

  

#### 함수

##### public CoroutineStream Pause();

- 진행 중인 작업을 멈춥니다.

```csharp
    CSPlayer.CoroutineStream()
      .Pause();
```

##### public CoroutineStream Resume();

- 작업을 다시 계속 진행합니다.

```csharp
    CSPlayer.CoroutineStream()
     .Pause();
```

##### public CoroutineStream Stop();

- 작업을 중지합니다.

```csharp
    CSPlayer.CoroutineStream()
      .Stop();
```

##### public CoroutineStream Append(params IEnumerator[] coroutine);

- 동시에 실행할 한개 이상의 코루틴을 추가합니다.

```csharp
  CSPlayer.CoroutineStream()
    .Append(CoroutineA());
    
  CSPlayer.CoroutineStream()
    .Append(CoroutineA(), CoroutineB());
```

##### public CoroutineStream Interval(float seconds);

- 지연시간을 추가합니다.

```csharp
  CSPlayer.CoroutineStream()
    .Interval(10f);
```

##### public CoroutineStream Until(System.Func\<bool> predicate);

- 조건식이 참이 될 때까지 대기합니다.

```csharp
  CSPlayer.CoroutineStream()
    .Until(() => Input.GetKeyDown(KeyCode.Space));
```

##### public CoroutineStream While(System.Func\<bool> predicate);

- 조건식이 참인 동안 대기합니다.

```csharp
  CSPlayer.CoroutineStream()
    .While(() => Input.GetKey(KeyCode.Space));
```

##### public CoroutineStream Callback(System.Action callback);

- 콜백함수를 추가합니다.

```csharp
  CSPlayer.CoroutineStream()
    .Callback(() => Debug.Log("Hello World !!!));
```

##### public CoroutineStream OnComplete(System.Action callback);

- 등록된 모든 작업이 완료된 후에 호출될 함수를 등록합니다. 여러번 등록하는 경우 마지막에 등록한 함수만 호출 됩니다.

```csharp
  CSPlayer.CoroutineStream()
    .OnComplete(() => Debug.Log("Completed !!!));
```

##### public CoroutineStream Log(string message);

- 일반 로그를 출력합니다.

```csharp
  CSPlayer.CoroutineStream()
    .Log("Hi");
```

##### public CoroutineStream LogFormat(string message, params object[] args);

- 일반 로그를 포맷 형태로 출력합니다.

```csharp
  int x = 7;
  
  CSPlayer.CoroutineStream()
    .LogFormat("{0}", x);
```

##### public CoroutineStream LogWarning(string message);

- 경고 로그를 출력합니다.

```csharp
  CSPlayer.CoroutineStream()
    .LogWarning("What???");
```

##### public CoroutineStream LogWarningFormat(string message, params object[] args);

- 경고 로그를 포맷 형태로 출력합니다.

```csharp
  string message = "Who?";
  
  CSPlayer.CoroutineStream()
    .LogWarning("What??? {0}", message);
```

##### public CoroutineStream LogError(string message);

- 오류 로그를 출력합니다.

```csharp
  CSPlayer.CoroutineStream()
    .LogError("OMG !!!");
```

##### public CoroutineStream LogErrorFormat(string message, params object[] args);

- 오류 로그를 포맷 형태로 출력합니다.

```csharp
  int errorCode = 9;
  
  CSPlayer.CoroutineStream()
    .LogErrorFormat("OMG !!! - {0}", errorCode);
```

