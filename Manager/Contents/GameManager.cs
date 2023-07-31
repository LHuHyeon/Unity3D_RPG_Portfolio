using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

/*
[ GameManager 스크립트 ]
1. 모든 플레이어 정보를 GameManager에서 저장한다.
2. 장비 강화, 퀘스트 갱신, 세이브 등 기능을 담당한다.
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
    GameObject _player;

    HashSet<GameObject> _monsters = new HashSet<GameObject>();

    GameData _gameData = new GameData();
    public GameData SaveData { get { return _gameData; } set { _gameData = value; } }

    public bool isSaveLoad = false;

    public GameObject GetPlayer() { return _player; }

    public MonsterStat currentMonster;   // 전투 중인 몬스터
    public Vector3 defualtSpawn;        // 기본 스폰 장소

    public UI_PlayScene _playScene;

    public Dictionary<Define.Popup, bool> isPopups;     // 팝업 bool 관리

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
		get { return _gameData.MoveSpeed; }
		set { _gameData.MoveSpeed = value + addMoveSpeed; }
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

    // 퀘스트 목표 개수 반영
    public void QuestTargetCount(GameObject go)
    {
        if (CurrentQuest.Count == 0)
            return;

        if (go.GetComponent<MonsterStat>())
        {
            // 퀘스트 조건과 맞는지 id 확인
            foreach(QuestData questData in CurrentQuest)
            {
                if (questData.targetId == go.GetComponent<MonsterStat>().Id)
                {
                    questData.currnetTargetCount = Mathf.Clamp(++questData.currnetTargetCount, 0, questData.targetCount);
                    if (questData.currnetTargetCount == questData.targetCount)
                    {
                        string message = $"퀘스트 완료!\n<color=yellow>[{questData.titleName}]</color>\n\n\n\n\n\n\n\n\n";
                        Managers.UI.MakeSubItem<UI_Guide>().SetInfo(message, Color.green);
                    }
                        
                    return;
                }
            }
        }
    }

#region Equipment

    // 강화 비용 계산
    public int EquipmentUpgradeGold(EquipmentData equipment)
    {
        // 강화 금액 : 아이템 판매 가격 + ((판매 가격 / 2) * 강화 횟수)
        int gold = equipment.itemPrice + (int)((equipment.itemPrice / 2) * (equipment.upgradeCount));
        return gold;
    }

    // 강화 적용
    public void EquipmentUpgrade(EquipmentData equipment)
    {
        equipment.upgradeCount += 1;

        // 장비 종류 확인 후 적용
        if (equipment is WeaponItemData)
        {
            WeaponItemData weapon = equipment as WeaponItemData;

            weapon.addAttack = weapon.upgradeValue * weapon.upgradeCount;
        }
        else if (equipment is ArmorItemData)
        {
            ArmorItemData armor = equipment as ArmorItemData;

            armor.addDefnece = armor.upgradeValue * armor.upgradeCount;
            armor.addHp = (armor.upgradeValue * 5) * armor.upgradeCount;
            armor.addMp = (armor.upgradeValue * 5) * armor.upgradeCount;
        }
    }

    // 업그레이드 일정 수치 넘으면 Mesh 적용
    public void UpgradeMeshEffect(EquipmentData equipment)
    {
        // 해당 무기 오브젝트 찾기
        GameObject weaponObj = (equipment as WeaponItemData).charEquipment;
        if (weaponObj.IsFakeNull() == true)
            weaponObj = (Managers.Data.Item[equipment.id] as WeaponItemData).charEquipment;

        // 객체 안에 자식들 삭제
        foreach(Transform child in weaponObj.transform)
                Managers.Resource.Destroy(child.gameObject);

        // 레벨 충족이 안되면 종료 
        if (equipment.upgradeCount < 6)
            return;

        // 무기 객체 자식으로 이펙트 배치
        string path = "Effect/Upgrade/UpgradeEffect_" + equipment.upgradeCount;
        GameObject effectObj = Managers.Resource.Instantiate(path, weaponObj.transform);

        PSMeshRendererUpdater meshRenderer = effectObj.GetComponent<PSMeshRendererUpdater>();
        meshRenderer.MeshObject = weaponObj;
        meshRenderer.UpdateMeshEffect(weaponObj);
    }

    public int addDefense = 0;
    public int addHp = 0;
    public int addMp = 0;
    public int addMoveSpeed = 0;
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

#endregion

    // 상호작용 시 옮겨지는 슬롯에 관한 함수
    public Action<UI_InvenItem> _getSlotInteract;
    public void GetSlotInteract(UI_InvenItem invenSlot)
    {
        if (_getSlotInteract.IsNull() == false)
            _getSlotInteract.Invoke(invenSlot);
    }

    // 해당 키 스킬 반환 
    public SkillData GetSkill(Define.KeySkill keySkill)
    {
        SkillData skill;
        if (SkillBarList.TryGetValue(keySkill, out skill) == false)
            return null;

        return skill;
    }

    // 공격 받을때
    public void OnAttacked(MonsterStat attacker, int addDamge = 0)
    {
        OnAttacked(attacker.Attack + addDamge);
    }

    public void OnAttacked(int damage)
    {
        Hp -= Mathf.Max(0, damage - Defense);

        if (Hp <= 0){
            Hp = 0;
            OnDead();
        }
    }

    GameObject tempPlayer;
    public void OnDead()
    {
        _player.GetComponent<PlayerController>().State = Define.State.Die;
        tempPlayer = _player;
        _player = null;

        Managers.UI.ShowPopupUI<UI_DiePopup>();
    }

    // 플레이어 부활
    public void OnResurrection(float health)
    {
        _player = tempPlayer;
        _player.GetComponent<PlayerController>().State = Define.State.Idle;

        Hp = (int)(MaxHp * health);
        Mp = (int)(MaxMp * health);
    }

    public void OnUpdate()
    {
        StatRecovery();

    }

    // Hp, Mp 재생 ( 5초마다 10 회복 )
    float healthTime = 0f;
    void StatRecovery()
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

    float saveTime = 0f;
    void OnSave()
    {
        saveTime += Time.deltaTime;
        if (saveTime >= 3f)
            SaveGame();
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

        switch(type){
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

        switch(type){
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
        switch(GetWorldObjectType(go)){
            case Define.WorldObject.Monster:
                {
                    if (_monsters.Contains(go)){ // 존재 여부 확인
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
