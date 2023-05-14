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

    // 현재 가지고 있는 스킬
    public List<SkillData> Skills = new List<SkillData>();

    // 스킬 : 스킬 능력, 레벨, 흭득 횟수
    // 등록된 스킬 : 스킬 능력
    // 퀘스트 : 퀘스트 내용, 완료 보상, 클리어 유/무
}

// 컨텐츠에서 사용될 매니저 (플레이어, 몬스터 등..)
public class GameManager
{
    GameObject _player;

    HashSet<GameObject> _monsters = new HashSet<GameObject>();

    public GameObject GetPlayer() { return _player; }

    public Action<int> OnSpawnEvent;

    GameData _gameData = new GameData();
    public GameData SaveData { get { return _gameData; } set { _gameData = value; } }

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
		get { return _gameData.MaxHp; }
		set { _gameData.MaxHp = value; }
	}

    public int Mp
	{
		get { return _gameData.Mp; }
		set { _gameData.Mp = Mathf.Clamp(value, 0, MaxMp); }
	}

    public int MaxMp
	{
		get { return _gameData.MaxMp; }
		set { _gameData.MaxMp = value; }
	}

    public int Attack
	{
		get 
        {
            // TODO : 공격력 설정
            // 장착 무기 데미지, STR 확인
            Debug.Log("STR" + STR);
            return (STR * 5);
        }
		private set {}
	}

    public int Defense
    {
        get 
        {
            // TODO : 장비 장착 시 증가
            return 0;
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
		set { _gameData.HpPoint = value; }
	}

    public int MpPoint
	{
		get { return _gameData.MpPoint; }
		set { _gameData.MpPoint = value; }
	}

    public Dictionary<Define.KeySkill, SkillData> SkillBarList
    {
        get { return _gameData.SkillBarList; }
        set { _gameData.SkillBarList = value; }
    }

    public List<SkillData> Skills
    {
        get { return _gameData.Skills; }
        set { _gameData.Skills = value; }
    }

    public void OnUpdate()
    {
        
    }

	public void RefreshExp()
	{
        int level = Level;

        while (true)
        {
            LevelData stat;
            
            // 해당 Key에 Value가 존재 하는지 여부
            if (Managers.Data.Level.TryGetValue(level + 1, out stat) == false)
            {
                Debug.Log("만렙 입니다!");
                break;
            }

            // 경험치가 다음 레벨 경험치보다 작은지 확인
            if (Exp < stat.totalExp)
                break;
            
            level++;
        }

        if (level != Level)
        {
            Level = level;
            
            RefreshStat(Level);
            Debug.Log("Level UP!!");
        }
	}

    public void RefreshStat(int level)
    {
        LevelData stat = Managers.Data.Level[level];

        TotalExp = stat.totalExp;
        StatPoint = stat.statPoint;
        MaxStatPoint += StatPoint;
        MaxHp = stat.maxHp;
        Hp = MaxHp;
        MaxMp = stat.maxMp;
        Mp = MaxMp;
    }

    // 해당 키 스킬 반환 (스킬 ui 완성되면 사용)
    // public SkillData GetSkill(Define.KeySkill keySkill)
    // {
    //     SkillData skill;
    //     if (SkillBarList.TryGetValue(keySkill, out skill) == false)
    //         return null;

    //     if (Skills.Contains(skill) == false)
    //         return null;

    //     return skill;
    // }

    // TODO : 스킬 ui 완성되면 삭제
    // 스킬 Id로 데이터 불러오기
    public SkillData GetSkill(int skillId)
    {
        SkillData skill;
        if (Managers.Data.Skill.TryGetValue(skillId, out skill) == false)
            return null;

        return skill;
    }

    public void OnAttacked(Stat attacker)
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
        // TODO : (정리)
        // Application.persistentDataPath을 변수 선언할때 바로 초기화하여 넣을 수 없음.
        // 무조건 Awake, Start에서 넣어줘야 함.
        _savePath = $"{Application.persistentDataPath}/SaveData.json";

        _player = GameObject.FindWithTag("Player");
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

    // 캐릭터 소환
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
