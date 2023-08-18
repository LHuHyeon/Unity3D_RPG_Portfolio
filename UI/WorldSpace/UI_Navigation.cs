using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 퀘스트 장소 알려주는 네비게이션
public class UI_Navigation : UI_Base
{
    Vector3 targetPos;
    float endScan = 7f;

    public void SetInfo(Vector3 pos)
    {
        targetPos = pos;
        gameObject.SetActive(true);
    }

    void Update()
    {
        if (Managers.Game.GetPlayer().IsNull() == true || gameObject.activeSelf == false)
            return;

        Vector3 dir = targetPos - Managers.Game.GetPlayer().transform.position;
        if (dir.magnitude <= endScan)
            gameObject.SetActive(false);

        transform.position = Managers.Game.GetPlayer().transform.position + (dir.normalized * 2f);
    }
}
