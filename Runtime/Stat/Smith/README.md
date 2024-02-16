# Smith 메뉴얼

## 소개
Stat을 베이스로한 장비 생성 코드 입니다.  게임마다 테이블을 구성하는 방식이 달라 대부분 구현부를 ISmithContainer로 분리해서 실제 코드는 얼마 되지 않습니다.


## 설치

https://github.com/cookapps-devops/pxp-foundation


## 사용법

1. EquipmentBase<T1, T2, T3> 클래스를 상속받아 게임에서 사용할 장비 데이타 클래스를 만듭니다.
2. ISmithContainer<T1, T2, T3>를 상속받아 인터페이스를 구현하고 Smith 클래스 생성시 인자로 전달합니다.
3. Smith.Create<T> 함수를 통해 EquipmentBase를 상속받은 클래스 객체를 받아 사용하면 됩니다.

사용 예제 코드

```csharp
using PxP.Stat.Smith;

/// 스탯
public enum STAT { NONE, ATK, HP, DEF, SPD, CRIT_RATE, CRIT_DMG, STUN, DODGE, ACC, SKILL_DMG,
    CRIT_RATE_RES, CRIT_DMG_RES, STUN_RES, DODGE_RES, ACC_RES, SKILL_DMG_RED }

/// 등급
public enum GRADE { NONE, COMMON, UNCOMMON, MAGIC, RARE, UNIQUE, EPIC, LEGEND, ETERNAL,
    MYTHIC, GOLDEN }

/// 파츠
public enum PART { NONE, WEAPON, HEAD, GLOVES, SHOULDER, ARMOR, ARMS, BELT, PANTS, BOOTS,
    RING, EARRING, AMULET }

[Serializable]
public class Equipment : EquipmentBase<PART, GRADE, STAT>
{
    public Equipment(PART part, GRADE grade, int level, List<KeyAndModifier<STAT>> defaultModifiers,
        List<KeyAndModifier<STAT>> extraModifiers)
        : base(part, grade, level, defaultModifiers, extraModifiers)
    {
    }
}

public class SmithManager : MonoManager<SmithManager>, ISmithContainer<PART, GRADE, STAT>
{
    private void Start()
    {
        var smith = new Smith<PART, GRADE, STAT>(this);
        var equipment = smith.Create<Equipment>();

        Debug.Log(equipment);
    }

    public int GetLevel() { ... }
    public IReadOnlyList<PART> GetPartTable() { ... }
    public IReadOnlyList<GRADE> GetGradeTable() { ... }
    public IReadOnlyList<float> GetProbabilityTable() { ... }
    public List<KeyAndModifier<STAT>> GetDefaultModifiers(PART part, GRADE grade) { ... }
    public List<KeyAndModifier<STAT>> GetExtraModifiers(PART part, GRADE grade) { ... }
    public EquipmentBase<PART, GRADE, STAT> CreateEquipment(PART part, GRADE grade, int level,
            List<KeyAndModifier<STAT>> defaultModifiers, List<KeyAndModifier<STAT>> extraModifiers) { ... }
}
```