# Manager 메뉴얼

## 소개
게임 내부에서 제공하는 다양한 서비스들의 라이프 사이클 관리와 접근을 용이하기 위한 클래스 입니다.

## 사용법

1. Manager 또는 Manager<T>를 상속받는 컴포넌트를 만든다.
2. 해당 컴포넌트를 게임오브젝트에 추가한다.
3. 추가로 제작한 Manager 또한 위 게임오브젝트에 추가하거나 자식 게임오브젝트를 만들어 추가한다.
4. 최상위 게임오브젝트의 Manager들 중 하나를 Initialize를 호출한다. (해당 게임오브젝트와 모든 자식 오브젝트의 초기화가 진행된다.)
5. 어느 Manager에서든 Get\<T>를 통해 다른 Manager를 접근할 수 있다. (Manager\<T>를 사용한다면 싱글톤 처럼 직접 접근이 가능하다.)

사용 예제 코드

```csharp
using UnityEngine;
using DarkNaku.Foundation;

public class MasterManager : Manager // 또는 Manager<MasterManager>
{
    // 초기화 시점에 호출
    protected override void OnInitialize()
    {
        Debug.LogFormat("OnInitialize : {0}", GetType());
    }

    // 자신이 속한 모든 Manager 그룹의 초기화가 끝난 뒤에 호출
    protected override void OnStart()
    {
        Debug.LogFormat("OnStart : {0}", GetType());
    }

    // 해제 시 호출
    protected override void OnDispose()
    {
        Debug.LogFormat("OnUninitialize : {0}", GetType());
    }
}
```