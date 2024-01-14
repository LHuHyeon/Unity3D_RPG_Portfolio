using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * File :   UI_Navigation.cs
 * Desc :   퀘스트 장소를 알려주는 네비게이션 UI
 *
 & Functions
 &  SetInfo()   - 기능 설정
 &  Update()    - 플레이어 위치에서 퀘스트 방향 가르키기
 *
 */

public class UI_Navigation : UI_Base
{
    private Vector3     targetPos;      // 목표 위치
    private float       endScan = 7f;   // 목표 위치 스캔 거리

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
