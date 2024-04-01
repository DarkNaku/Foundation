# Messenger 메뉴얼

## 소개
UniRx의 MessageBroker와 유사한 하지만 단순하게 메세지를 전달 받기 위한 메시지 선언 없이 사용하기 위한 라이브러리 입니다.


## 설치

https://github.com/cookapps-devops/pxp-foundation


## 사용법

1. Messenger.AddListener로 수신할 메세지와 콜백을 지정합니다.
2. Messenger.RemoverListener로 수신을 종료합니다.

예제 코드를 통해 확인해보실 수 있습니다.

사용 예제 코드

```csharp
using UnityEngine;
using DarkNaku.Messenger;

public class MessengerTest : MonoBehaviour
{
    public enum TestMessageType { Test1 }
    
    private void OnEnable()
    {
        Messenger<TestMessageType, int, string, bool>.AddListener(TestMessageType.Test1, OnBroadcast);
        MessengerString<int, string, bool>.AddListener("TestMessage", OnBroadcast2);
    }

    private void OnDisable()
    {
        Messenger<TestMessageType, int, string, bool>.RemoveListener(TestMessageType.Test1, OnBroadcast);
        MessengerString<int, string, bool>.RemoveListener("TestMessage", OnBroadcast2);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Messenger<TestMessageType, int, string, bool>.Broadcast(TestMessageType.Test1, 123, "Hello, World!", true);
        }
        
        if (Input.GetKeyDown(KeyCode.A))
        {
            MessengerString<int, string, bool>.Broadcast("TestMessage", 456, "Hello, Universe!", false);
        }
    }
    
    private void OnBroadcast(int value, string value2, bool value3)
    {
        Debug.Log($"OnBroadcast: {value}");
        Debug.Log($"OnBroadcast: {value2}");
        Debug.Log($"OnBroadcast: {value3}");
    }
    
    private void OnBroadcast2(int value, string value2, bool value3)
    {
        Debug.Log($"OnBroadcast: {value}");
        Debug.Log($"OnBroadcast: {value2}");
        Debug.Log($"OnBroadcast: {value3}");
    }
}
```