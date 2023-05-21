using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.Networking;

public class DataManager
{
    const string URL = "https://docs.google.com/spreadsheets/d/1wGzHHrNKnq8LYkQHWN3DWJLY5zRBllqKT69KmzN5oWo/export?format=csv&gid=";

    public StartData Start { get; private set; }
    public Dictionary<int, LevelData> Level { get; private set; }
    public Dictionary<int, SkillData> Skill { get; private set; }
    public Dictionary<int, ItemData> Item { get; private set; }
    public Dictionary<int, List<int>> DropItem { get; private set; }
    public Dictionary<int, GameObject> Monster { get; private set; }
    // public Dictionary<int, TextData> Texts { get; private set; }

    // TODO : 보기 좋게 바꾸기 ( 아니면 안쓰기 )
    public bool[] isDataRequest = new bool[] {false, false, false, false, false, false, false, false};

    public void Init()
    {
        Item = new Dictionary<int, ItemData>();
    }

    // 게임 시작 시 호출 (GameScene)
    public IEnumerator DataRequest(string dataNumber)
    {
        UnityWebRequest www = UnityWebRequest.Get(URL+dataNumber);

        yield return www.SendWebRequest();

        string data = www.downloadHandler.text;

        switch(dataNumber)
        {
            case Define.StartNumber:
                StartRequest(data);
                isDataRequest[0] = true;
                break;
            case Define.LevelNumber:
                LevelRequest(data);
                isDataRequest[1] = true;
                break;
            case Define.SkillNumber:
                SkillRequest(data);
                isDataRequest[2] = true;
                break;
            case Define.UseItemNumber:
                UseItemRequest(data);
                isDataRequest[3] = true;
                break;
            case Define.WeaponItemNumber:
                WeaponItemRequest(data);
                isDataRequest[4] = true;
                break;
            case Define.ArmorItemNumber:
                ArmorItemRequest(data);
                isDataRequest[5] = true;
                break;
            case Define.DropItemNumber:
                DropItemRequest(data);
                isDataRequest[6] = true;
                break;
            case Define.MonsterNumber:
                MonsterRequest(data);
                isDataRequest[7] = true;
                break;
        }
    }

#region 데이터 파싱

    void StartRequest(string data)
    {
        Start = new StartData();

        string[] lines = data.Split("\n");
        string[] row = lines[1].Replace("\r", "").Split(',');

        Start = new StartData()
        {
            Id = int.Parse(row[0]),
            totalExp = int.Parse(row[1]),
            exp = int.Parse(row[2]),
            level = int.Parse(row[3]),
            maxHp = int.Parse(row[4]),
            maxMp = int.Parse(row[5]),
            STR = int.Parse(row[6]),
            MoveSpeed = int.Parse(row[7]),
            LUK = int.Parse(row[8]),
        };
    }

    void LevelRequest(string data)
    {
        Level = new Dictionary<int, LevelData>();

        string[] lines = data.Split("\n");
        for(int y = 1; y < lines.Length; y++)
        {
            string[] row = lines[y].Replace("\r", "").Split(',');
            if (row.Length == 0)
				continue;
			if (string.IsNullOrEmpty(row[0]))
				continue;

            LevelData levelData = new LevelData()
            {
                level = int.Parse(row[0]),
                totalExp = int.Parse(row[1]),
                statPoint = int.Parse(row[2]),
                maxHp = int.Parse(row[3]),
                maxMp = int.Parse(row[4]),
            };

            Level.Add(levelData.level, levelData);
        }
    }

    void SkillRequest(string data)
    {
        Skill = new Dictionary<int, SkillData>();

        string[] lines = data.Split("\n");

        for(int y = 1; y < lines.Length; y++)
        {
            string[] row = lines[y].Replace("\r", "").Split(',');
            
            if (row.Length == 0)
				continue;
			if (string.IsNullOrEmpty(row[0]))
				continue;

            // 스킬 정보 1~6
            SkillData skillData = new SkillData()
            {
                skillId = int.Parse(row[0]),
                skillName = row[1],
                minLevel = int.Parse(row[2]),
                skillCoolDown = int.Parse(row[3]),
                skillConsumMp = int.Parse(row[4]),
                discription = row[5],
            };

            // Sprite 6
            skillData.skillSprite = Managers.Resource.Load<Sprite>("Art/UI/Skill/"+row[6]);

            // 공격력 7
            List<int> powerList = new List<int>();
            foreach(string attackNumber in row[7].Split("|"))
                powerList.Add(int.Parse(attackNumber));

            skillData.powerList = powerList;

            Skill.Add(skillData.skillId, skillData);
        }
    }

    void UseItemRequest(string data)
    {
        string[] lines = data.Split("\n");
        for(int y = 1; y < lines.Length; y++)
        {
            string[] row = lines[y].Replace("\r", "").Split(',');
            if (row.Length == 0)
				continue;
			if (string.IsNullOrEmpty(row[0]))
				continue;

            UseItemData useItem = new UseItemData()
            {
                id = int.Parse(row[0]),
                itemName = row[1],
                useType = (Define.UseType)int.Parse(row[2]),
                itemGrade = (Define.itemGrade)int.Parse(row[3]),
                useValue = int.Parse(row[4]),
                itemPrice = int.Parse(row[5]),
                itemDesc = row[6],
                itemMaxCount = 99,
                itemIcon = Managers.Resource.Load<Sprite>("Art/UI/Item/Use/"+row[7]),
                itemObject = Managers.Resource.Load<GameObject>("Prefabs/Object/Use/"+row[8]),
            };

            Item.Add(useItem.id, useItem);
        }
    }

    void WeaponItemRequest(string data)
    {
        string[] lines = data.Split("\n");
        for(int y = 1; y < lines.Length; y++)
        {
            string[] row = lines[y].Replace("\r", "").Split(',');
            if (row.Length == 0)
				continue;
			if (string.IsNullOrEmpty(row[0]))
				continue;

            WeaponItemData weaponItem = new WeaponItemData()
            {
                id = int.Parse(row[0]),
                itemName = row[1],
                weaponType = (Define.WeaponType)int.Parse(row[2]),
                itemGrade = (Define.itemGrade)int.Parse(row[3]),
                minLevel = int.Parse(row[4]),
                attack = int.Parse(row[5]),
                upgradeValue = int.Parse(row[6]),
                itemPrice = int.Parse(row[7]),
                itemDesc = row[8],
                itemMaxCount = 1,
                itemIcon = Managers.Resource.Load<Sprite>("Art/UI/Item/Weapon/"+row[9]),
                itemObject = Managers.Resource.Load<GameObject>("Prefabs/Object/Weapon/"+row[10]),
            };

            Item.Add(weaponItem.id, weaponItem);
        }
    }

    void ArmorItemRequest(string data)
    {
        string[] lines = data.Split("\n");
        for(int y = 1; y < lines.Length; y++)
        {
            string[] row = lines[y].Replace("\r", "").Split(',');
            if (row.Length == 0)
				continue;
			if (string.IsNullOrEmpty(row[0]))
				continue;

            ArmorItemData armorItem = new ArmorItemData()
            {
                id = int.Parse(row[0]),
                itemName = row[1],
                armorType = (Define.ArmorType)int.Parse(row[2]),
                itemGrade = (Define.itemGrade)int.Parse(row[3]),
                minLevel = int.Parse(row[4]),
                upgradeValue = int.Parse(row[5]),
                itemPrice = int.Parse(row[6]),
                defnece = int.Parse(row[7]),
                hp = int.Parse(row[8]),
                mp = int.Parse(row[9]),
                moveSpeed = int.Parse(row[10]),
                itemDesc = row[11],
                itemMaxCount = 1,
                itemIcon = Managers.Resource.Load<Sprite>("Art/UI/Item/Armor/"+row[12]),
                itemObject = Managers.Resource.Load<GameObject>("Prefabs/Object/Armor/"+row[13]),
            };

            Item.Add(armorItem.id, armorItem);
        }
    }

    void DropItemRequest(string data)
    {
        DropItem = new Dictionary<int, List<int>>();

        string[] lines = data.Split("\n");

        for(int y = 1; y < lines.Length; y++)
        {
            string[] row = lines[y].Replace("\r", "").Split(',');
            
            if (row.Length == 0)
				continue;
			if (string.IsNullOrEmpty(row[0]))
				continue;

            List<int> itemDatas = new List<int>();
            foreach(string itemdata in row[1].Split("|"))
                itemDatas.Add(int.Parse(itemdata));

            DropItem.Add(int.Parse(row[0]), itemDatas);
        }
    }

    void MonsterRequest(string data)
    {
        Monster = new Dictionary<int, GameObject>();

        string[] lines = data.Split("\n");
        for(int y = 1; y < lines.Length; y++)
        {
            string[] row = lines[y].Replace("\r", "").Split(',');
            if (row.Length == 0)
				continue;
			if (string.IsNullOrEmpty(row[0]))
				continue;

            GameObject monsterObject = Managers.Resource.Load<GameObject>("Prefabs/Monster/"+row[8]);
            MonsterStat monsterStat = monsterObject.GetOrAddComponent<MonsterStat>();

            monsterStat.Id = int.Parse(row[0]);
            monsterStat.Name = row[1];
            monsterStat.MaxHp = int.Parse(row[2]);
            monsterStat.Attack = int.Parse(row[3]);
            monsterStat.MoveSpeed = int.Parse(row[4]);
            monsterStat.DropExp = int.Parse(row[5]);
            monsterStat.DropGold = int.Parse(row[6]);
            monsterStat.DropItemId = int.Parse(row[7]);

            Monster.Add(monsterStat.Id, monsterObject);
        }
    }

#endregion
}
