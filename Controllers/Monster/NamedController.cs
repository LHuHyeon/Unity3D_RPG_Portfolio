using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
[ 네임드 컨트롤러 스크립트 ]
1. 패턴 : 일반 공격 2회 -> 강력한 근접 공격 1회
*/

public class NamedController : MonsterController
{
    int attackCount = 0;    // 공격 횟수 (3번 하면 스킬 진행)

    GameObject attackRangeObj;

    [SerializeField]
    Vector3 skillRangeScale; // 스킬 공격 범위 크기

    [SerializeField]
    Vector3 skillRangePos;

    public override void Init()
    {
        base.Init();

        monsterType = Define.MonsterType.Named;

        skillRangeScale = new Vector3(1, 0.0001f, 2f);
        skillRangePos = new Vector3(0, 0, 1.5f);
    }

    protected override void OnAttackEvent()
    {
        base.OnAttackEvent();
        attackCount++;
    }

    protected override void ExitAttack()
    {
        if (attackCount == 2)
        {
            // 스킬 공격 범위 생성
            attackRangeObj = Managers.Resource.Instantiate("Object/BoxAttackRange", this.transform);
            attackRangeObj.transform.localPosition = skillRangePos;
            attackRangeObj.transform.localScale = skillRangeScale;
            attackRangeObj.GetOrAddComponent<AttackRange>().SetInfo(_stat, false);

            State = Define.State.Skill;
            anim.CrossFade("SKILL", 0.1f, -1, 0);
        }
        else
            base.ExitAttack();
    }

    public void OnSkillEvent()
    {
        Managers.Resource.Destroy(attackRangeObj);
        attackCount = 0;
    }

    public void ExitSkillEvent()
    {
        State = Define.State.Idle;
    }

    protected override void UpdateHit() {}
}
