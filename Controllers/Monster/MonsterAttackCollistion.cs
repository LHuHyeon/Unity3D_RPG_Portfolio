using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * File :   MonsterAttackCollistion.cs
 * Desc :   몬스터 콜라이더 공격
 *
 & Functions
 &  [Public]
 &  : IsCollider()      - 콜라이더 여부 설정
 &
 &  [Private]
 &  : OnTriggerEnter()  - 플레이어와 접촉 시 데미지 반영
 *
 */

public class MonsterAttackCollistion : MonoBehaviour
{
    public int          damage;

    [SerializeField]
    private BoxCollider boxCollider;

    public void IsCollider(bool isActive) { boxCollider.enabled = isActive; }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") == true)
            Managers.Game.OnAttacked(damage);
    }
}
