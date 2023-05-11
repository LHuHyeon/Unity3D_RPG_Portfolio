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
    // public Dictionary<int, TextData> Texts { get; private set; }

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
                break;
            case Define.LevelNumber:
                LevelRequest(data);
                break;
        }
    }

#region 데이터 파싱

    void StartRequest(string data)
    {
        Start = new StartData();

        string[] lines = data.Split("\n");
        string[] row = lines[1].Replace("\r", "").Split(',');

        StartData startData = new StartData()
        {
            Id = int.Parse(row[0]),
            exp = int.Parse(row[1]),
            level = int.Parse(row[2]),
            maxHp = int.Parse(row[3]),
            maxMp = int.Parse(row[4]),
            STR = int.Parse(row[5]),
            Speed = int.Parse(row[6]),
            LUK = int.Parse(row[7]),
        };
    }

    void LevelRequest(string data)
    {
        Level = new Dictionary<int, LevelData>();

        string[] lines = data.Split("\n");
        for(int y = 1; y < lines.Length; y++)
        {
            Debug.Log(lines[y]);
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

#endregion
}
