using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * File :   PlayerAttackCollistion.cs
 * Desc :   플레이어 전방에 Collider를 활성화하여 접촉된 몬스터에게 데미지 반영
 *
 & Functions
 &  : OnEnable()            - 활성화    시 0.1f 후 비활성화 (DelayActiveFalse() Invoke 호출)
 &  : OnDisable()           - 비활성화  시 스킬 콤보 체크 및 콜라이더 사이즈 초기화
 &  : OnTriggerEnter()      - 몬스터 접촉 시 데미지 반영 
 &
 &  [Private]
 &  : DelayActiveFalse()    - 비활성화 딜레이
 &  : BasicColliderSize()   - 콜라이더 기본 사이즈 초기화
 *
 */

public class PlayerAttackCollistion : MonoBehaviour
{
    private int                 skillIndex = 0;     // 스킬 콤보 공격력 List index

    private CapsuleCollider     capsuleCollider;
    
    [SerializeField]
    private PlayerController    player;

    void Start()
    {
        capsuleCollider = GetComponent<CapsuleCollider>();

        skillIndex = 0;

        gameObject.SetActive(false);
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

    // Invoke 호출
    private void DelayActiveFalse()
    {
        gameObject.SetActive(false);
    }

    // 기본 콜라이더 사이즈
    private void BasicColliderSize()
    {
        capsuleCollider.center = new Vector3(0, 0, 0.4f);
        capsuleCollider.radius = 1.2f;
        capsuleCollider.height = 2.4f;
    }
}
