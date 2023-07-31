using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
[ 플레이어 공격 Collider 스크립트 ]
1. 플레이어 전방에 원형 Collider를 활성화하여 닿은 몬스터에게 데미지를 준다.
*/

public class AttackCollistion : MonoBehaviour
{
    private CapsuleCollider capsuleCollider;
    
    [SerializeField]
    PlayerController player;

    // 콤보 스킬 공격력 리스트 index
    int skillIndex = 0;

    void Start()
    {
        capsuleCollider = GetComponent<CapsuleCollider>();

        skillIndex = 0;

        gameObject.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Monster"))
        {
            if (player.State == Define.State.Skill)
            {
                if (player.currentSkill.powerList.Contains(skillIndex) == false)
                    skillIndex = 0;

                // 스킬 공격
                int skillDamage = player.currentSkill.powerList[skillIndex] * (Managers.Game.Attack / 2);
                other.GetComponent<MonsterStat>().OnAttacked(skillDamage);
            }
            else
                other.GetComponent<MonsterStat>().OnAttacked(); // 기본 공격
        }   
    }

    void OnEnable()
    {
        Invoke("DelayActiveFalse", 0.1f);
    }

    void OnDisable()
    {
        BasicColliderSize();

        // 마지막 스킬 공격이라면 index 초기화 
        if (player.currentSkill.IsNull() == false)
        {
            if (skillIndex == player.currentSkill.powerList.Count - 1)
                skillIndex = 0;
            else
                skillIndex++;
        }
    }

    void DelayActiveFalse()
    {
        gameObject.SetActive(false);
    }

    void BasicColliderSize()
    {
        capsuleCollider.center = new Vector3(0, 0, 0.4f);
        capsuleCollider.radius = 1.2f;
        capsuleCollider.height = 2.4f;
    }
}
