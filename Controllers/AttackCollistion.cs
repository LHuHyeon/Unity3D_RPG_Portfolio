using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

        gameObject.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Monster"))
        {
            Debug.Log("Monster Hit!");

            if (player.State == Define.State.Skill)
            {
                // 스킬 공격
                int skillDamage = player.currentSkill.powerList[skillIndex] * Managers.Game.STR;
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
        if (player.currentSkill != null)
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
