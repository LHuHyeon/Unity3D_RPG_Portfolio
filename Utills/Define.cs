using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Define : MonoBehaviour
{
    public enum KeySkill
    {
        Q,
        W,
        E,
        R,
        A,
        S,
        D,
    }

    public enum ShopType
    {
        Unknown,
        Used,
        Equipment,
    }

    public enum StatType
    {
        Hp,
        Mp,
        Attack,
        Defence,
        MoveSpeed,
    }

    public enum WeaponType
    {
        Unknown,
        Sword,
    }

    public enum ArmorType
    {
        Unknown,
        Helm,       // 모자
        Chest,      // 갑옷
        Pants,      // 바지
        Boots,      // 신발
        Gloves,     // 장갑
        Necklace,   // 목걸이
        Ring,       // 반지
        MaxCount,
    }

    public enum UseType
    {
        Unknown,
        Hp,
        Mp,
    }

    public enum itemGrade
    {
        Common,     // 기본
        Rare,       // 레어
        Epic,       // 에픽
        Legendary,  // 레전드
    }

    public enum ItemType
    {
        Unknown,
        Use,
        Armor,
        Weapon,
        ETC,
    }

    public enum SlotType
    {
        Unknown,
        Inven,      // 인벤토리 슬롯
        Quest,      // 퀘스트 보상 슬롯
        Equipment,  // 장비 아이템 슬롯
        Skill,      // 스킬 슬롯 (Scene, Popup 둘다)
        UseItem,    // 사용 아이템 슬롯 (Scene)
    }

    // 캐릭터 상태
    public enum State
    {
        Moving,
        DiveRoll,
        Idle,
        Hit,
        Die,
        Attack,
        Skill,
    }

    public enum WorldObject
    {
        Unknown,
        Player,
        Monster,
    }

    public enum Layer
    {
        Monster = 8,
        Ground = 9,
        Block = 10,
        Npc = 11,
    }

    public enum Scene
    {
        Unknown,
        Login,
        Loby,
        Game,
    }

    public enum Sound
    {
        Bgm,
        Effect,
        MaxCount,   // MaxCount를 마지막 자리에 둠으로 써 해당 enum의 최대 개수(int)가 저장됨.
    }

    public enum UIEvent
    {
        Enter,
        Exit,
        Click,
        BeginDrag,
        Drag,
        EndDrag,
        Drop,
    }

    public enum MouseEvent
    {
        LeftPress,
        RightPress,
        LeftDown,
        RightDown,
        LeftUp,
        RightUp,
        LeftClick,
        RightClick,
    }

    public enum CameraMode
    {
        QuarterView,    // 디아블로 게임 같은 시점
    }

    public const string StartNumber = "0";
    public const string LevelNumber = "2089499917";
    public const string SkillNumber = "160891494";
    public const string UseItemNumber = "1334508722";
    public const string WeaponItemNumber = "271156662";
    public const string ArmorItemNumber = "1624069194";
    public const string DropItemNumber = "563258933";
    public const string MonsterNumber = "1085946242";
}
