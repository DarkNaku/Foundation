# Messenger 메뉴얼

## 소개
UniRx의 MessageBroker와 유사한 형태로 메세지를 전달 받기 위한 라이브러리 입니다.


## 설치

https://github.com/DarkNaku/Foundation


## 사용법

1. Messenger.AddListener로 수신할 메세지와 콜백을 지정합니다.
2. Messenger.RemoverListener로 수신을 종료합니다.
3. Messenger.Broadcast로 메시지를 발생 시킵니다.

예제 코드를 통해 확인해보실 수 있습니다.

사용 예제 코드

```csharp
using UnityEngine;
using DarkNaku.Messenger;

public struct TestMessage
{
    public readonly int a;
}

public class MessengerTest : MonoBehaviour
{
    public enum TestMessageType { Test1 }
    
    private void OnEnable()
    {
        Messenger.AddListener<TestMessage>(OnBroadcast);
    }

    private void OnDisable()
    {
        Messenger.RemoveListener<TestMessage>(OnBroadcast);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Messenger.Broadcast(new TestMessage() { a = 1 });
        }
    }
    
    private void OnMessage(TestMessage msg)
    {
        Debug.Log($"OnMessage: {msg.a}");
    }
}
```