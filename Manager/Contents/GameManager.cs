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

    // 스킬바에 등록된 리스트
    public Dictionary<Define.KeySkill, SkillData> SkillBarList = new Dictionary<Define.KeySkill, SkillData>();

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

    // 스킬 : 스킬 능력, 레벨, 흭득 횟수
    // 등록된 스킬 : 스킬 능력
    // 퀘스트 : 퀘스트 내용, 완료 보상, 클리어 유/무
}

// 컨텐츠에서 사용될 매니저 (플레이어, 몬스터 등..)
public class GameManager
{
    GameData _gameData = new GameData();
    public GameData SaveData { get { return _gameData; } set { _gameData = value; } }

    GameObject _player;

    HashSet<GameObject> _monsters = new HashSet<GameObject>();

    public GameObject GetPlayer() { return _player; }

    public Action<int> OnSpawnEvent;

    public UI_PlayScene _playScene;

    public bool isInventory = false;
    public bool isEquipment = false;
    public bool isSkillUI = false;
    public bool isTalk = false;
    public bool isQuest = false;
    private bool isInteract = false;
    public bool IsInteract
    {
        get { return isInteract; }
        set {
            // 상호작용 중이면 Popup UI 끄기
            isInteract = value;
            
            if (isInteract == true)
            {
                isInventory = false;
                isEquipment = false;
                isSkillUI = false;
            }
        }
    }

    public void StopPlayer()
    {
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
            if (CurrentWeapon != null)
                return (STR * 2) + CurrentWeapon.attack;
            
            return (STR * 2);
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

    public Dictionary<Define.KeySkill, SkillData> SkillBarList
    {
        get { return _gameData.SkillBarList; }
        set { _gameData.SkillBarList = value; }
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

    public void OnUpdate()
    {
        
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
                addDefense += armorData.defnece;
                addHp += armorData.hp;
                addMp += armorData.mp;
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

    // 해당 키 스킬 반환 
    public SkillData GetSkill(Define.KeySkill keySkill)
    {
        SkillData skill;
        if (SkillBarList.TryGetValue(keySkill, out skill) == false)
            return null;

        return skill;
    }

    // 공격 받을때
    public void OnAttacked(MonsterStat attacker)
    {
        Hp -= Mathf.Max(0, attacker.Attack - Defense);

        if (Hp <= 0){
            Hp = 0;
            OnDead();
        }
    }

    public void OnDead()
    {
        // TODO : 재시작
    }

    public void Init()
    {
        _savePath = $"{Application.persistentDataPath}/SaveData.json";

        CurrentArmor = new Dictionary<Define.ArmorType, ArmorItemData>();

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
    }

    // 캐릭터 소환 (주소)
    public GameObject Spawn(Define.WorldObject type, string path, Transform parent = null)
    {
        GameObject go = Managers.Resource.Instantiate(path, parent);

        switch(type){
            case Define.WorldObject.Monster:
                _monsters.Add(go);
                if (OnSpawnEvent != null)
                    OnSpawnEvent.Invoke(1);
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
                    OnSpawnEvent.Invoke(1);
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
                            OnSpawnEvent.Invoke(-1);
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
}
