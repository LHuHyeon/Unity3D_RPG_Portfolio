using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GoogleSheetManager
{
    const string URL = "https://docs.google.com/spreadsheets/d/1wGzHHrNKnq8LYkQHWN3DWJLY5zRBllqKT69KmzN5oWo/export?format=csv&gid=";

    public Dictionary<int, StartData> Start { get; private set; }

    public void Init()
    {
    }

    public IEnumerator DataRequest()
    {
        UnityWebRequest www = UnityWebRequest.Get(URL+Define.StartNumber);

        yield return www.SendWebRequest();

        string data = www.downloadHandler.text;
        Debug.Log(data);

        // [ 데이터 받기 ] (TODO : 블로그 기록하기)
        Start = new Dictionary<int, StartData>();

        string[] lines = data.Split("\n");
        for(int y=1; y < lines.Length; y++)
        {
            string[] row = lines[y].Replace("\r", "").Split(',');
            if (row.Length == 0)
				continue;
			if (string.IsNullOrEmpty(row[0]))
				continue;

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

            Start.Add(startData.Id, startData);
        }

        Debug.Log($"Start Count : {Start.Count}");

        for(int i=1; i<=Start.Count; i++)
        {
            if (Start.TryGetValue(i, out StartData value) == false)
                Debug.Log($"{i} : false");
            else
                Debug.Log(value.level + " " + value.exp + " " + value.maxHp);
        }
    }
}
