using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        if (attackCount == 2)
        {
            // 스킬 공격 범위 생성
            attackRangeObj = Managers.Resource.Instantiate("Object/AttackRange", this.transform);
            attackRangeObj.transform.localPosition = skillRangePos;
            attackRangeObj.transform.localScale = skillRangeScale;
            attackRangeObj.GetComponent<AttackRange>().SetInfo(_stat);

            State = Define.State.Skill;
        }

        base.OnAttackEvent();
        attackCount++;
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
