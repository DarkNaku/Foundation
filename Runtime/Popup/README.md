# Popup 메뉴얼

## 소개
Popup을 관리하는 코드 입니다. 팝업 단위로 켄버스를 가지고 있으며 싱글톤으로 제작되었습니다. Hierarachy에 존재하는 상태로 동작할 수 있고 런타임에 팝업을 등록하여 사용할 수도 있습니다. IPopupTransition 인터페이스를 상속받은 컴포넌트를 핸들러 오브젝트에 추가하여 팝업 등장 및 퇴장 연출을 추가할 수 있습니다.


## 설치

https://github.com/cookapps-devops/pxp-foundation


## 사용법

1. 게임 오브젝트에 Popup 클래스를 추가합니다.(없더라도 런타임에 호출하면 자동으로 게임오브젝트가 생성됩니다.)
2. PopupHandler 클래스를 상속받은 팝업 핸들러를 만들고 위에서 만든 게임 오브젝트의 자식으로 추가합니다.
3. PopupHandler가 붙어 있는 게임오브젝트 이름을 키로하여 Popup.Show, Popup.Hide API를 통해 열고 닫을 수 있습니다.

예제 코드를 받아 확인해보실 수 있습니다.

사용 예제 코드

```csharp
using PxP.Popup;

public class APopup : PopupHandler
{
    [SerializeField] private Button _buttonClose;
    [SerializeField] private Button _buttonShow;

    protected override void OnWillEnter(object param)
    {
        Debug.Log("A 팝업이 열렸습니다.");
    }

    private void OnEnable()
    {
        _buttonClose.onClick.AddListener(OnButtonClicked);
        _buttonShow.onClick.AddListener(OnButtonShowClicked);
    }

    private void OnDisable()
    {
        _buttonClose.onClick.RemoveListener(OnButtonClicked);
        _buttonShow.onClick.RemoveListener(OnButtonShowClicked);
    }
    
    private void OnButtonClicked()
    {
        Popup.Hide(this);
    }
    
    private void OnButtonShowClicked()
    {
        Popup.Show("BPopup", onWillHide: (result) => Debug.Log("B 팝업이 닫혔습니다."));
    }
}
```