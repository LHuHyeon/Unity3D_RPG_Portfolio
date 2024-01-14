using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * File :   UI_HpBar.cs
 * Desc :   Monster 위에 생성되는 Hp바 UI
 *
 & Functions
 &  Init()          - 초기 설정 
 &  FixedUpdate()   - 객체 상단 위치 고정 및 카메라 바라보도록 회전
 *
 */

public class UI_HpBar : UI_Base
{
    private MonsterStat     _stat;

    enum GameObjects
    {
        HpBar
    }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        Bind<GameObject>(typeof(GameObjects));

        _stat = transform.parent.GetComponent<MonsterStat>();
        gameObject.SetActive(false);

        return true;
    }

    void FixedUpdate()
    {
        // 체력 설정
        Transform parent = transform.parent;
        transform.position = parent.position + Vector3.up * (parent.GetComponent<Collider>().bounds.size.y);
        GetObject((int)GameObjects.HpBar).transform.rotation = Camera.main.transform.rotation;

        float ratio = (float)_stat.Hp / _stat.MaxHp;
        
        GetObject((int)GameObjects.HpBar).GetComponent<Slider>().value = ratio;
    }
}
