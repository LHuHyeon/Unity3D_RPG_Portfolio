using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

/*
 * File :   GameManager.cs
 * Desc :   1인 게임이기 때문에 이 곳에서 플레이어의 데이터를 관리한다.
 *          새로고침, 세이브 등을 담당
 *          [ Rookiss의 MMORPG Game Part 3 참고. ]
 *
 & Functions
 &  [Public]
 &  : GetPlayer()           - 플레이어 반환
 &  : StopPlayer()          - 플레이어 정지
 &  : RefreshExp()          - 경험치 레벨업 확인
 &  : RefreshStat()         - 레벨에 맞는 스탯 가져오기
 &  : RefreshQuest()        - 퀘스트 클리어 확인
 &  : RefreshAllEquipment() - 모든 장비의 스탯 적용
 &  : RefreshArmor()        - 방어구 스탯 적용
 &  : GetSlotInteract()     - 슬롯 상호작용 (인벤토리 <-> NPC간 사용)
 &  : OnAttacked()          - 플레이어 피격
 &  : OnDead()              - 플레이어 사망
 &  : OnResurrection()      - 플레이어 부활
 &  : UpdateStatRecovery()  - 체력/마나 재생
 &  : Init()                - 초기 설정
 &  : Spawn()               - 캐릭터 스폰 (플레이어 or 몬스터)
 &  : GetWorldObjectType()  - 오브젝트 타입 반환
 &  : Despawn()             - 캐릭터 삭제
 &  : IsSaveLoad()          - 세이브 불러올 수 있는지 확인
 &  : SaveGame()            - 세이브 진행
 &  : LoadGame()            - 세이브 로드
 &  : ToInvenItem()         - 인벤토리 세이브 진행
 &  : FromInvenItem()       - 인벤토리 세이브 로드
 &  : ToDictionary()        - 딕셔너리 세이브 진행
 &  : FromDictionary()      - 딕셔너리 세이브 로드
 &  : Clear()               - 초기화
 *
 */

[Serializable]
public class DataDictionary<TKey,TValue>
{
    public TKey Key;
    public TValue Value;
}

[Serializable]
public class GameData
{
    public string Name;

	public float Exp;
    public float TotalExp;

	public int Level;
    public int Hp;
    public int MaxHp;
    public int Mp;
    public int MaxMp;
    public int STR;
    public int MoveSpeed;
    public int LUK;
    public int StatPoint;
    public int MaxStatPoint;
    public int HpPoint;
    public int MpPoint;

    public int Gold;        // 골드 (게임 재화)

    public Vector3 CurrentPos = Vector3.zero;

    // 캐릭터 기본 부위
    public Dictionary<Define.DefaultPart, SkinnedData> DefaultPart = new Dictionary<Define.DefaultPart, SkinnedData>();

    // Scene 스킬바에 등록된 리스트
    public Dictionary<Define.KeySkill, SkillData> SkillBarList = new Dictionary<Define.KeySkill, SkillData>();

    // Scene 소비아이템바에 등록된 리스트
    public Dictionary<int, UseItemData> UseItemBarList = new Dictionary<int, UseItemData>();

    // 현재 장착한 장비
    public Dictionary<Define.ArmorType, ArmorItemData> CurrentArmor = new Dictionary<Define.ArmorType, ArmorItemData>();

    // 현재 인벤토리 아이템
    public Dictionary<int, ItemData> InvenItem = new Dictionary<int, ItemData>();

    // 인벤 Save 용도 
    public Dictionary<int, UseItemData> InvenUseItem = new Dictionary<int, UseItemData>();
    public Dictionary<int, WeaponItemData> InvenWeaponItem = new Dictionary<int, WeaponItemData>();
    public Dictionary<int, ArmorItemData> InvenArmorItem = new Dictionary<int, ArmorItemData>();

    // Dictionary 세이브
    public List<DataDictionary<Define.DefaultPart, SkinnedData>> DefaultPartData = new List<DataDictionary<Define.DefaultPart, SkinnedData>>();
    public List<DataDictionary<Define.KeySkill, SkillData>> SkillBarListData = new List<DataDictionary<Define.KeySkill, SkillData>>();
    public List<DataDictionary<int, UseItemData>> UseItemBarListData = new List<DataDictionary<int, UseItemData>>();
    public List<DataDictionary<Define.ArmorType, ArmorItemData>> CurrentArmorData = new List<DataDictionary<Define.ArmorType, ArmorItemData>>();
    public List<DataDictionary<int, UseItemData>> InvenUseItemData = new List<DataDictionary<int, UseItemData>>();
    public List<DataDictionary<int, WeaponItemData>> InvenWeaponItemData = new List<DataDictionary<int, WeaponItemData>>();
    public List<DataDictionary<int, ArmorItemData>> InvenArmorItemData = new List<DataDictionary<int, ArmorItemData>>();

    // 현재 무기
    public WeaponItemData CurrentWeapon;

    // 현재 흭득한 스킬
    public List<SkillData> CurrentSkill = new List<SkillData>();

    // 현재 퀘스트
    public List<QuestData> CurrentQuest = new List<QuestData>();

    // 클리어한 퀘스트
    public List<QuestData> ClearQuest = new List<QuestData>();
}

// 컨텐츠에서 사용될 매니저 (플레이어, 몬스터 등..)
public class GameManager
{
    private GameData            _gameData = new GameData();
    public GameData             SaveData { get { return _gameData; } set { _gameData = value; } }

    private GameObject          _player;
    private HashSet<GameObject> _monsters = new HashSet<GameObject>();

    public bool                 isSaveLoad = false;     // 세이브 불러왔는지 여부

    public MonsterStat          currentMonster;         // 전투 중인 몬스터
    public Vector3              defualtSpawn;           // 기본 스폰 장소

    public UI_PlayScene         _playScene;             // 게임 플레이 Scene UI

    public Dictionary<Define.Popup, bool> isPopups;     // 팝업 bool 관리

    // NPC와 상호작용 여부
    private bool isInteract = false;
    public bool IsInteract
    {
        get { return isInteract; }
        set {
            // NPC와 상호작용하면 플레이어만의 Popup창 끄기
            if (value == true)
            {
                // 장비창, 인벤창, 스킬창, 퀘스트창 False
                isPopups[Define.Popup.Equipment] = false;
                isPopups[Define.Popup.Inventory] = false;
                isPopups[Define.Popup.SkillUI] = false;
                isPopups[Define.Popup.Quest] = false;
            }

            isInteract = value;
        }
    }

    public GameObject GetPlayer() { return _player; }

    public void StopPlayer()
    {
        if (_player.IsNull() == true)
            return;
            
        PlayerController player = _player.GetComponent<PlayerController>();
        player.State = Define.State.Idle;
    }

    public string Name
	{
		get { return _gameData.Name; }
		set { _gameData.Name = value; }
	}

    public float Exp
	{
		get { return _gameData.Exp; }
		set { _gameData.Exp = value; RefreshExp(); }
	}

    public float TotalExp
	{
		get { return _gameData.TotalExp; }
		set { _gameData.TotalExp = value; }
	}
	
	public int Level
	{
		get { return _gameData.Level; }
		set { _gameData.Level = value; }
	}

    public int Hp
	{
		get { return _gameData.Hp; }
		set { _gameData.Hp = Mathf.Clamp(value, 0, MaxHp); }
	}

    public int MaxHp
	{
		get { return _gameData.MaxHp + (HpPoint * 20) + addHp; }
		set { _gameData.MaxHp = value; }
	}

    public int Mp
	{
		get { return _gameData.Mp; }
		set { _gameData.Mp = Mathf.Clamp(value, 0, MaxMp); }
	}

    public int MaxMp
	{
		get { return _gameData.MaxMp + (MpPoint * 20) + addMp; }
		set { _gameData.MaxMp = value; }
	}

    public int Attack
	{
		get 
        {
            // 무기가 있으면 ( 무기 공격력 + 무기 추가 공격력 )
            if (CurrentWeapon.IsNull() == false)
                return (int)(STR * 0.5) + CurrentWeapon.attack + CurrentWeapon.addAttack;
            
            return (int)(STR * 0.5);
        }
		private set {}
	}

    public int Defense
    {
        get 
        {
            return addDefense;
        }
		private set {}
    }

    public int STR
	{
		get { return _gameData.STR; }
		set { _gameData.STR = value; }
	}

    public int MoveSpeed
	{
		get { return _gameData.MoveSpeed + addMoveSpeed; }
		set { _gameData.MoveSpeed = value; }
	}

    public int LUK
	{
		get { return _gameData.LUK; }
		set { _gameData.LUK = value; }
	}

    public int Gold
	{
		get { return _gameData.Gold; }
		set { _gameData.Gold = value; }
	}

    public int StatPoint
	{
		get { return _gameData.StatPoint; }
		set { _gameData.StatPoint = value; }
	}

    public int MaxStatPoint
	{
		get { return _gameData.MaxStatPoint; }
		set { _gameData.MaxStatPoint = value; }
	}

    public int HpPoint
	{
		get { return _gameData.HpPoint; }
		set { _gameData.HpPoint = value; Hp = MaxHp; }
	}

    public int MpPoint
	{
		get { return _gameData.MpPoint; }
		set { _gameData.MpPoint = value; Mp = MaxMp; }
	}

    public Vector3 CurrentPos
	{
		get { return _gameData.CurrentPos; }
		set { _gameData.CurrentPos = value; }
	}

    public Dictionary<Define.DefaultPart, SkinnedData> DefaultPart
    {
        get { return _gameData.DefaultPart; }
        set { _gameData.DefaultPart = value; }
    }

    public Dictionary<Define.KeySkill, SkillData> SkillBarList
    {
        get { return _gameData.SkillBarList; }
        set { _gameData.SkillBarList = value; }
    }

    public Dictionary<int, UseItemData> UseItemBarList
    {
        get { return _gameData.UseItemBarList; }
        set { _gameData.UseItemBarList = value; }
    }

    public Dictionary<Define.ArmorType, ArmorItemData> CurrentArmor
    {
        get { return _gameData.CurrentArmor; }
        set { _gameData.CurrentArmor = value; }
    }

    public Dictionary<int, ItemData> InvenItem
    {
        get { return _gameData.InvenItem; }
        set { _gameData.InvenItem = value; }
    }

    public WeaponItemData CurrentWeapon
    {
        get { return _gameData.CurrentWeapon; }
        set { _gameData.CurrentWeapon = value; }
    }

    public List<SkillData> CurrentSkill
    {
        get { return _gameData.CurrentSkill; }
        set { _gameData.CurrentSkill = value; }
    }

    public List<QuestData> CurrentQuest
    {
        get { return _gameData.CurrentQuest; }
        set { _gameData.CurrentQuest = value; }
    }

    public List<QuestData> ClearQuest
    {
        get { return _gameData.ClearQuest; }
        set { _gameData.ClearQuest = value; }
    }

    // 추가 스탯
    public int addDefense = 0;
    public int addHp = 0;
    public int addMp = 0;
    public int addMoveSpeed = 0;

    public void OnUpdate()
    {
        UpdateStatRecovery();
    }

    // Exp 증가시 레벨업 확인
	public void RefreshExp()
	{
        // 다음 레벨 확인
        if (Managers.Data.Level.ContainsKey(Level + 1) == false)
        {
            Debug.Log("만렙 입니다!");
            return;
        }

        // 경험치가 다음 레벨 경험치보다 작은지 확인
        if (Exp < TotalExp)
            return;
        
        // 레벨 업
        RefreshStat(++Level);
        _player.GetComponent<PlayerController>().LevelUpEffect();
        Managers.UI.MakeSubItem<UI_Guide>().SetInfo($"Level Up!! \n({Level}) \n\n\n\n\n\n\n\n\n", Color.yellow);
        Debug.Log("Level UP!!");
	}

    // 레벨 스탯 가져오기
    public void RefreshStat(int level)
    {
        Debug.Log("RefreshStat Level : " + level);
        LevelData stat = Managers.Data.Level[level];

        Exp = 0;
        TotalExp = stat.totalExp;
        StatPoint += stat.statPoint;
        MaxStatPoint += stat.statPoint;
        MaxHp = stat.maxHp;
        Hp = MaxHp;
        MaxMp = stat.maxMp;
        Mp = MaxMp;

        _playScene.RefreshUI();
    }

    // 클리어한 퀘스트가 있는지 확인
    public void RefreshQuest()
    {
        for(int i=0; i<CurrentQuest.Count; i++)
        {
            if (CurrentQuest[i].isClear == true)
            {
                ClearQuest.Add(CurrentQuest[i]);
                CurrentQuest.RemoveAt(i);
            }
        }
    }

    // 전체 장비 스탯 적용
    public void RefreshAllEquipment()
    {
        // 장비 스탯 전체 적용
        ArmorItemData armorData;
        addDefense = 0;
        addHp = 0;
        addMp = 0;
        addMoveSpeed = 0;
        for(Define.ArmorType i=0; i<Define.ArmorType.MaxCount; i++)
        {
            if (CurrentArmor.TryGetValue(i, out armorData) == true)
            {
                // 장비가 있으면 더하고, 없으면 빼야됨.
                addDefense += armorData.defnece + armorData.addDefnece;
                addHp += armorData.hp + armorData.addHp;
                addMp += armorData.mp + armorData.addMp;
                addMoveSpeed += armorData.moveSpeed;
            }
        }
    }

    // 장비 스탯 적용
    public void RefreshArmor(ArmorItemData armorItem, bool isStat)
    {
        if (armorItem.IsNull() == false)
        {
            if (isStat == true)
            {
                addDefense += armorItem.defnece;
                addHp += armorItem.hp;
                addMp += armorItem.mp;
                addMoveSpeed += armorItem.moveSpeed;
            }
            else
            {
                addDefense -= armorItem.defnece;
                addHp -= armorItem.hp;
                addMp -= armorItem.mp;
                addMoveSpeed -= armorItem.moveSpeed;
            }
        }
    }

    // 플레이어가 NPC와 상호작용할 때 슬롯 기능을 각각에 맞게 수행
    public Action<UI_InvenSlot> _getSlotInteract;
    public void GetSlotInteract(UI_InvenSlot invenSlot)
    {
        if (_getSlotInteract.IsNull() == false)
            _getSlotInteract.Invoke(invenSlot);
    }

    // 공격 받을때
    public void OnAttacked(MonsterStat attacker, int addDamge = 0)
    {
        OnAttacked(attacker.Attack + addDamge);
    }

    public void OnAttacked(int damage)
    {
        Hp -= Mathf.Max(0, damage - Defense);

        if (Hp <= 0)
        {
            Hp = 0;
            OnDead();
        }
    }

    public void OnDead()
    {
        if (_player.GetComponent<PlayerController>().State != Define.State.Die)
        {
            _player.GetComponent<PlayerController>().State = Define.State.Die;
            Managers.UI.ShowPopupUI<UI_DiePopup>();
        }
    }

    // 플레이어 부활
    public void OnResurrection(float health)
    {
        _player.GetComponent<PlayerController>().State = Define.State.Idle;

        Hp = (int)(MaxHp * health);
        Mp = (int)(MaxMp * health);
    }

    // Hp, Mp 재생 ( 5초마다 10 회복 )
    float healthTime = 0f;
    void UpdateStatRecovery()
    {
        // 둘다 Full이면 재생 X
        if (Hp == MaxHp && Mp == MaxMp)
            return;

        healthTime += Time.deltaTime;
        if (healthTime >= 5f)
        {
            Hp += 10;
            Mp += 10;

            healthTime = 0;
        }
    }

    public void Init()
    {
        _savePath = $"{Application.persistentDataPath}/SaveData.json";

        // 첫 시작일 경우
        if (isSaveLoad == false && Managers.Data.Start.IsNull() == false)
        {
            Debug.Log("GameManager Init : StartData True!");
            StartData data = Managers.Data.Start;

            TotalExp = data.totalExp;
            Exp = data.exp;
            Level = data.level;
            MaxHp = data.maxHp;
            Hp = MaxHp;
            MaxMp = data.maxMp;
            Mp = MaxMp;
            STR = data.STR;
            MoveSpeed = data.MoveSpeed;
            LUK = data.LUK;
            
            Gold = data.gold;
        }
        else if (Managers.Data.Start.IsNull() == true)
            Name = "NoName";

        MoveSpeed = 5;

        isPopups = new Dictionary<Define.Popup, bool>();
        for(int i=1; i<(int)Define.Popup.Max; i++)
            isPopups.Add((Define.Popup)i, false);
    }

    // 캐릭터 소환 (주소)
    public Action<Transform, int> OnSpawnEvent;
    public GameObject Spawn(Define.WorldObject type, string path, Transform parent = null)
    {
        GameObject go = Managers.Resource.Instantiate(path, parent);

        switch(type)
        {
            case Define.WorldObject.Monster:
                _monsters.Add(go);
                if (OnSpawnEvent.IsNull() == false)
                    OnSpawnEvent.Invoke(parent, 1);
                break;
            case Define.WorldObject.Player:
                _player = go;
                break;
            default:
                Debug.Log("GameManager : Null Type");
                break;
        }

        return go;
    }

    // 캐릭터 소환 (객체)
    public GameObject Spawn(Define.WorldObject type, GameObject obj, Transform parent = null)
    {
        GameObject go = Managers.Resource.Instantiate(obj, parent);

        switch(type)
        {
            case Define.WorldObject.Monster:
                _monsters.Add(go);
                if (OnSpawnEvent.IsNull() == false)
                    OnSpawnEvent.Invoke(parent, 1);
                break;
            case Define.WorldObject.Player:
                _player = go;
                break;
            default:
                Debug.Log("GameManager : Null Type");
                break;
        }

        return go;
    }

    // 타입 확인
    public Define.WorldObject GetWorldObjectType(GameObject go)
    {
        BaseController bc = go.GetComponent<BaseController>();
        if (bc.IsNull() == true)
            return Define.WorldObject.Unknown;

        return bc.WorldObjectType;
    }

    // 캐릭터 삭제
    public void Despawn(GameObject go)
    {
        switch(GetWorldObjectType(go))
        {
            case Define.WorldObject.Monster:
                {
                    if (_monsters.Contains(go)) // 존재 여부 확인
                    { 
                        _monsters.Remove(go);
                        if (OnSpawnEvent.IsNull() == false)
                        {
                            OnSpawnEvent.Invoke(go.transform.parent, -1);
                        }
                    }
                }
                break;
            case Define.WorldObject.Player:
                {
                    if (_player == go)
                    {
                        _player = null;
                        return;
                    }
                }
                break;
        }

        Managers.Resource.Destroy(go);
    }

    #region Save & Load	
	public string _savePath;

    public bool IsSaveLoad()
    {
        if (File.Exists(_savePath) == false)
			return false;
        
        Debug.Log($"Save Data True : {_savePath}");

        return true;
    }

	public void SaveGame()
	{
        if (Managers.Scene.CurrentScene.SceneType == Define.Scene.Game)
            CurrentPos = _player.transform.position;

        _gameData.DefaultPartData = ToDictionary<Define.DefaultPart, SkinnedData>(DefaultPart);
        _gameData.SkillBarListData = ToDictionary<Define.KeySkill, SkillData>(SkillBarList);
        _gameData.UseItemBarListData = ToDictionary<int, UseItemData>(UseItemBarList);
        _gameData.CurrentArmorData = ToDictionary<Define.ArmorType, ArmorItemData>(CurrentArmor);

        ToInvenItem();

		string jsonStr = JsonUtility.ToJson(Managers.Game.SaveData, true);

        File.WriteAllText(_savePath, jsonStr);

		Debug.Log($"Save Game Completed : {_savePath}");
	}

	public bool LoadGame()
	{
		if (IsSaveLoad() == false)
			return false;

		string fileStr = File.ReadAllText(_savePath);
		GameData data = JsonUtility.FromJson<GameData>(fileStr);
		if (data.IsNull() == false)
		{
            data.DefaultPart = FromDictionary<Define.DefaultPart, SkinnedData>(data.DefaultPartData);
            data.SkillBarList = FromDictionary<Define.KeySkill, SkillData>(data.SkillBarListData);
            data.UseItemBarList = FromDictionary<int, UseItemData>(data.UseItemBarListData);
            data.CurrentArmor = FromDictionary<Define.ArmorType, ArmorItemData>(data.CurrentArmorData);

            FromInvenItem<UseItemData>(data.InvenUseItemData, data);
            FromInvenItem<ArmorItemData>(data.InvenArmorItemData, data);
            FromInvenItem<WeaponItemData>(data.InvenWeaponItemData, data);

			Managers.Game.SaveData = data;
		}

		Debug.Log($"Save Game Loaded : {_savePath}");

        isSaveLoad = true;
		return true;
	}

    #region Save Inven Dictionary

    void ToInvenItem()
    {
        _gameData.InvenUseItem.Clear();
        _gameData.InvenArmorItem.Clear();
        _gameData.InvenWeaponItem.Clear();

        foreach(int key in InvenItem.Keys)
        {
            ItemData item = InvenItem[key];
            if (item is UseItemData)
                _gameData.InvenUseItem.Add(key, item as UseItemData);
            else if (item is ArmorItemData)
                _gameData.InvenArmorItem.Add(key, item as ArmorItemData);
            else if (item is WeaponItemData)
                _gameData.InvenWeaponItem.Add(key, item as WeaponItemData);
        }

        _gameData.InvenUseItemData = ToDictionary<int, UseItemData>(_gameData.InvenUseItem);
        _gameData.InvenArmorItemData = ToDictionary<int, ArmorItemData>(_gameData.InvenArmorItem);
        _gameData.InvenWeaponItemData = ToDictionary<int, WeaponItemData>(_gameData.InvenWeaponItem);
    }

    void FromInvenItem<TValue>(List<DataDictionary<int, TValue>> itemData, GameData gameData) where TValue : ItemData
    {
        Dictionary<int, TValue> dict = FromDictionary<int, TValue>(itemData);
        foreach(int key in dict.Keys)
        {
            gameData.InvenItem.Add(key, dict[key]);
        }
    }

    #endregion

    #region Save Dictionary

    public List<DataDictionary<TKey, TValue>> ToDictionary<TKey, TValue>(Dictionary<TKey, TValue> dicData)
    {
        List<DataDictionary<TKey, TValue>> dataList = new List<DataDictionary<TKey, TValue>>();
        DataDictionary<TKey, TValue> dictionaryData;
        foreach (TKey key in dicData.Keys)
        {
            if (dicData[key].IsNull() == true)
                continue;
                
            dictionaryData = new DataDictionary<TKey, TValue>();
            dictionaryData.Key = key;
            dictionaryData.Value = dicData[key];
            dataList.Add(dictionaryData);
        }

        return dataList;
    }

    public Dictionary<TKey, TValue> FromDictionary<TKey, TValue>(List<DataDictionary<TKey, TValue>> dataList)
    {
         Dictionary<TKey, TValue> returnDictionary = new Dictionary<TKey, TValue>();

        for (int i = 0; i < dataList.Count; i++)
        {
            DataDictionary<TKey, TValue> dictionaryData = dataList[i];
            returnDictionary[dictionaryData.Key] = dictionaryData.Value;
        }

        return returnDictionary;
    }

    #endregion

	#endregion

    public void Clear()
    {
        _monsters.Clear();
        currentMonster = null;
        OnSpawnEvent = null;
    }
}
