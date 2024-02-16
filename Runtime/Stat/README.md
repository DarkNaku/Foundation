# Stat 메뉴얼

## 소개
RPG 게임에서 보통 사용하는 케릭터 스탯 관리 코드 입니다. 케릭터의 스탯 이외에도 기본값과 변화를 분리하여 처리해야 하는 부분에 사용할 수 있습니다.

## 설치

https://github.com/cookapps-devops/pxp-foundation


## 사용법

사용 예제 코드

```csharp
using PxP.Stat;

public enum STAT { POWER, HEALTH, DEFENCE, AGILITY, WISDOM, LUCKY }

var stats = new CharacterStats<STAT>();

stats.Add(STAT.POWER, 10);
stats.Add(STAT.HEALTH, 15);
stats.Add(STAT.DEFENCE, 8);
stats.Add(STAT.AGILITY, 5);
stats.Add(STAT.WISDOM, 3);
stats.Add(STAT.LUCKY, 4);

stats.Log("초기값");

Stats.AddModifier(STAT.POWER, new Modifier(ModifierType.PercentAdd, 0.1f, source: this));
Stats.AddModifier(STAT.HEALTH, new Modifier(ModifierType.Sum, 13, source: this));

Stats.Log("수정후");
        
Stats.RemoveModifierFromSource(this);
        
Stats.Log("제거후");
```

## 클래스 

### `Stat<T>`

#### 속성

##### `public float InitialValue { get; }`

설명: 생성자를 통해 최초 설정된 값을 가지고 있음. (읽기전용)

타입: float 

##### `public float BaseValue { get; set; }`

설명: 수정 사항들이 반영 되지 않은 값을 가지고 있음.

타입: float 

##### `public float Value { get; }`

설명: 모든 수정 사항들을 적용한 최종 결과값. (읽기전용)

타입: float 

##### `public float PermanentValue { get; }`

설명: 일시적인 수정사항을 제외하고 나머지 수정사항들을 적용한 결과값. (읽기전용)

타입: float 

##### `public T Key { get; }`

설명: 스탯을 고유하게 만들기 위해 사용하는 키값. 일반적으로 string 또는 enum 타입을 지정합니다. (읽기전용)

타입: 제네릭 

##### `public UnityEvent<Stat<T>> OnChangeValue { get; }`

설명: 스탯 수정 사항이 발생하는 경우 이벤트를 받기위해 사용합니다. (읽기전용)

타입: UnityEvent

##### `public CalculateMethod CustomCalculateMethod { get; set; }`

설명: 커스텀한 계산 방식을 적용하기 위한 델리게이트 속성입니다. 기본적으로 수정사항은 등록 순서와 상관없이 Sum, PercentAdd, PercentMultiply 순서로 계산됩니다. 다른 방법으로는 클래스를 상속받아 CalculateFinalValue 함수를 오버라이드 하는 방법도 있습니다.

타입: CalculateMethod

#### 생성자

##### `public Stat()`

설명: 초기값이 0f로 저장되고 Key는 default로 설정됩니다.

##### `public Stat(float initialValue, T key = default)`

설명: 초기값과 Key 값을 설정할 수 있습니다.

매개변수:
* `initialValue` (float): 초기값.
* `key` (T): 키값. 기본은 default.

#### 함수

##### `public void AddModifier(Modifier modifier)`

설명: 수정 사항을 추가합니다.

매개변수:
* `modifier` (Modifier): 수정 사항.

Returns: void

##### `public void RemoveModifier(Modifier modifier)`

설명: 수정 사항을 제거합니다.

매개변수:
* `modifier` (Modifier): 수정 사항.

Returns: void

##### `public void RemoveModifiersFromID(string id)`

설명: 수정 사항에 부여된 ID를 찾아 제거합니다.

매개변수:
* `id` (string): 수정 사항 ID.

Returns: void

##### `public void RemoveModifiersFromSource(object source)`

설명: 수정 사항에 부여된 object를 찾아 제거합니다. 스탯에 수정사항을 부여할 때 부여주체를 소스로 추가하기 때문에 수정 사항을 제거할 때 일반적으로 많이 사용합니다.

매개변수:
* `source` (object): 수정 사항을 부여한 주체.

Returns: void

### `Modifier`

#### 속성

##### `public Modifier Type { get; }`

설명: 수정 타입. 계산되는 방식을 지정한다. (읽기전용)

타입: Modifier

##### `public float Value { get; }`

설명: 수정에 적용할 수치. (읽기전용)

타입: float

##### `public bool IsTemporary { get; }`

설명: 일시적인 수정 사항 여부에 대한 표시. (읽기전용)

타입: bool

##### `public object Source { get; set; }`

설명: 수정 사항을 부여한 주체 등 수정사항들을 그룹화 하기 위한 속성.

타입: object 

##### `public string ID { get; set; }`

설명: 수정사항을 고유하게 구분하기 위한 문자열.

타입: string

#### 생성자

##### `public Modifier(ModifierType type, float value, bool isTemporary = false, object source = null, string id = "")`

설명: Source와 ID를 제외한 속성의 모든 값은 생성자에서 지정하며 수정할 수 없습니다.

매개변수:
* `type` (ModifierType): 수정타입.
* `value` (float): 수치.
* `isTemporary` (bool): 일시적용 여부.
* `source` (object): 소스.
* `id` (string): 고유 문자열.

### `CharacterStats<T>`

#### 속성

##### `public IReadOnlyDictionary<T, Stat<T>> All { get; }`

설명: 모든 수정사항.

타입: IReadOnlyDictionary<T, Stat<T>>

##### `public UnityEvent<CharacterStats<T>, Stat<T>> OnChangeStat { get; }`

설명: 수정 사항의 변동에 대한 이벤트.

타입: UnityEvent<CharacterStats<T>, Stat<T>>

##### `public Stat<T> this[T key] { get; }`

설명: Key를 통해 특정 Stat을 얻어 올 수 있습니다.

타입: Stat<T>

#### 함수

##### `public bool Contains(T key)`

설명: Key 보유 여부 확인.

매개변수:
* `key` (T): 스탯의 키.

Returns: bool

##### `public void Add(T key, float initialValue)`

설명: 스탯을 추가 합니다.

매개변수:
* `key` (T): 스탯의 키.
* `initialiValue` (float): 스탯 초기값.

Returns: void

##### `public void AddModifier(T key, Modifier modifier)`

설명: 수정 사항을 추가합니다.

매개변수:
* `key` (T): 스탯의 키.
* `modifier` (Modifier): 수정사항.

Returns: void

##### `public void RemoveModifier(T key, Modifier modifier)`

설명: 수정 사항을 제거합니다.

매개변수:
* `key` (T): 스탯의 키.
* `modifier` (Modifier): 수정사항.

Returns: void

##### `public void RemoveModifierFromID(string id)`

설명: 모든 Stat에서 같은 id를 가진 모든 수정을 제거합니다.

매개변수:
* `id` (string): 문자열 ID

Returns: void

##### `public void RemoveModifierFromSource(object source)`

설명: 모든 Stat에서 같은 소스를 가진 모든 수정을 제거합니다.

매개변수:
* `source` (object): 소스

Returns: void

##### `public void Log(string title)`

설명: 스탯들의 상태를 로그로 출력하는 함수입니다.

매개변수:
* `title` (string): 로그 구분을 위한 타이틀 문자열.

Returns: void