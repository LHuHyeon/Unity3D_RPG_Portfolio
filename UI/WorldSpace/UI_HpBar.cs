using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 몬스터 Hp바
public class UI_HpBar : UI_Base
{
    MonsterStat _stat;

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
        SetHpRatio(ratio);
    }

    void SetHpRatio(float ratio)
    {
        GetObject((int)GameObjects.HpBar).GetComponent<Slider>().value = ratio;
    }
}
