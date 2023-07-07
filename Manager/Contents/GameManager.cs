using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

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

    // 캐릭터 기본 부위
    public Dictionary<Define.DefaultPart, SkinnedData> DefaultPart = new Dictionary<Define.DefaultPart, SkinnedData>();

    // Scene 스킬바에 등록된 리스트
    public Dictionary<Define.KeySkill, SkillData> SkillBarList = new Dictionary<Define.KeySkill, SkillData>();

    // Scene 소비아이템바에 등록된 리스트
    public Dictionary<int, UI_UseItemSlot> UseItemBarList = new Dictionary<int, UI_UseItemSlot>();

    // 현재 장착한 장비
    public Dictionary<Define.ArmorType, ArmorItemData> CurrentArmor = new Dictionary<Define.ArmorType, ArmorItemData>();

    // 현재 무기
    public WeaponItemData CurrentWeapon;

    // 현재 인벤토리
    public List<UI_InvenItem> invenSlots;

    // 현재 퀘스트
    public List<QuestData> CurrentQuest = new List<QuestData>();

    // 클리어한 퀘스트
    public List<QuestData> ClearQuest = new List<QuestData>();
}

// 컨텐츠에서 사용될 매니저 (플레이어, 몬스터 등..)
public class GameManager
{
    GameData _gameData = new GameData();
    public GameData SaveData { get { return _gameData; } set { _gameData = value; } }

    GameObject _player;

    HashSet<GameObject> _monsters = new HashSet<GameObject>();

    public GameObject GetPlayer() { return _player; }

    public MonsterStat currentMonster;   // 전투 중인 몬스터

    public UI_PlayScene _playScene;

    public Vector3 beforeSpawnPos = Vector3.zero;      // 씬이 이동될 때 이동 되기전 위치를 저장

    public Dictionary<Define.Popup, bool> isPopups;     // 팝업 bool 관리

    private bool isInteract = false;
    public bool IsInteract
    {
        get { return isInteract; }
        set {
            // 상호작용 중이면 Popup UI 끄기
            isInteract = value;
            
            if (isInteract == true)
            {
                isPopups[Define.Popup.Inventory] = false;
                isPopups[Define.Popup.Equipment] = false;
                isPopups[Define.Popup.SkillUI] = false;
            }
        }
    }

    public void StopPlayer()
    {
        if (_player == null)
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
            if (CurrentWeapon != null)
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

    public Dictionary<int, UI_UseItemSlot> UseItemBarList
    {
        get { return _gameData.UseItemBarList; }
        set { _gameData.UseItemBarList = value; }
    }

    public Dictionary<Define.ArmorType, ArmorItemData> CurrentArmor
    {
        get { return _gameData.CurrentArmor; }
        set
        {
            _gameData.CurrentArmor = value;
        }
    }

    public WeaponItemData CurrentWeapon
    {
        get { return _gameData.CurrentWeapon; }
        set { _gameData.CurrentWeapon = value; }
    }

    public List<UI_InvenItem> InvenSlots
    {
        get { return _gameData.invenSlots; }
        set { _gameData.invenSlots = value; }
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
        if (equipment.upgradeCount > 5)
            UpgradeMeshEffect(equipment);

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
        // 객체 안에 자식들 삭제
        GameObject weaponObj = (equipment as WeaponItemData).charEquipment;
        foreach(Transform child in weaponObj.transform)
            Managers.Resource.Destroy(child.gameObject);

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
        if (armorItem != null)
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

    public void OnDead()
    {
        // TODO : 재시작
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

        if (Managers.Data.Start != null)
        {
            Debug.Log("GameManager Init : StartData True!");
            StartData data = Managers.Data.Start;

            Name = "NoName";
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

        MoveSpeed = 5;

        CurrentArmor = new Dictionary<Define.ArmorType, ArmorItemData>();

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
                if (OnSpawnEvent != null)
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
                if (OnSpawnEvent != null)
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
        if (bc == null)
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
                        if (OnSpawnEvent != null)
                        {
                            OnSpawnEvent.Invoke(go.transform.parent, -1);
                        }
                    }
                }
                break;
            case Define.WorldObject.Player:
                {
                    if (_player == go)
                        _player = null;
                }
                break;
        }

        Managers.Resource.Destroy(go);
    }

    #region Save & Load	
	public string _savePath;

	public void SaveGame()
	{
		string jsonStr = JsonUtility.ToJson(Managers.Game.SaveData);
		File.WriteAllText(_savePath, jsonStr);
		Debug.Log($"Save Game Completed : {_savePath}");
	}

	public bool LoadGame()
	{
		if (File.Exists(_savePath) == false)
			return false;

		string fileStr = File.ReadAllText(_savePath);
		GameData data = JsonUtility.FromJson<GameData>(fileStr);
		if (data != null)
		{
			Managers.Game.SaveData = data;
		}

		Debug.Log($"Save Game Loaded : {_savePath}");
		return true;
	}
	#endregion

    public void Clear()
    {
        _monsters.Clear();
        currentMonster = null;
        OnSpawnEvent = null;
    }
}
