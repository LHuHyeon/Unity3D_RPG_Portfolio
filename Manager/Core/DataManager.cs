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
    // public Dictionary<int, TextData> Texts { get; private set; }

    // TODO : 보기 좋게 바꾸기 ( 아니면 안쓰기 )
    public bool[] isDataRequest = new bool[] {false, false, false};

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
        }
    }

#region 데이터 파싱

    void StartRequest(string data)
    {
        Start = new StartData();

        string[] lines = data.Split("\n");
        string[] row = lines[1].Replace("\r", "").Split(',');

        Debug.Log("StartData\n[0] : " + lines[0] + "\n[1] : " + lines[1]);

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
        Debug.Log("LevelData\n[0] : " + lines[0] + "\n[1] : " + lines[1]);
        for(int y = 1; y < lines.Length; y++)
        {
            string[] row = lines[y].Replace("\r", "").Split(',');
            Debug.Log("row length : " + row.Length);
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
        Debug.Log("SkillData\n[0] : " + lines[0] + "\n[1] : " + lines[1]);

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
                skillCollTime = int.Parse(row[3]),
                skillConsumMp = int.Parse(row[4]),
                discription = row[5],
            };
            Debug.Log($"Id : {skillData.skillId}, Name : {skillData.skillName}");

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

#endregion
}
