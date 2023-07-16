using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
[ 범위 지속 공격 스크립트 ]
1. 범위 안에 타겟이 들어오면 데미지를 입힘
2. 나중에 다른 범위 공격 or 버프 등을 사용한다면 이 클래스를 부모로 수정하여 사용하기.
3. 현재 보스 지속공격으로 사용 중이다.
*/

public class AOEController : MonoBehaviour
{
    public int damage;

    GameObject target;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") == true)
            target = other.gameObject;
    }

    void OnTriggerStay(Collider other)
    {
        TargetDamage();
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") == false)
            return;

        target = null;
        currentTime = 0f;
    }

    float currentTime = 0f;
    float damageTime = 2f;
    void TargetDamage()
    {
        currentTime += Time.deltaTime;

        if (currentTime >= damageTime)
        {
            currentTime = 0;
            Managers.Game.OnAttacked(damage);
        }
    }
}
