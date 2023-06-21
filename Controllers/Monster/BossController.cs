using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossController : MonsterController
{
    /*
    1. 콤보 공격, 점프 착지 공격
    2. 체력이 반 깍이면 : 영혼 미사일(?) 스킬 추가
    3. 점프 착지 후 플레이어 띄우기
    */
    int attackCount = 0;    // 공격 횟수 (3번 하면 스킬 진행)

    GameObject attackRangeObj;

    public override void Init()
    {
        base.Init();

        monsterType = Define.MonsterType.Boss;
    }

    protected override void UpdateMoving()
    {
        // 공격 횟수가 3회 도달하면 점프 공격 진행
        // 거리 상관없이 사용해야 되므로 Moving 상태에서 진행
        if (attackCount >= 3)
        {
            if (_lockTarget != null)
            {
                attackCount = 0;
                StartCoroutine(JumpAttack());
                State = Define.State.Skill;
            }
        }
        
        base.UpdateMoving();
    }

    protected override void UpdateAttack()
    {
        // 공격 횟수가 2회 도달하면 콤보 공격 진행
        if (attackCount == 2)
        {
            anim.CrossFade("Combo Attack", 0.1f, -1, 0);
            State = Define.State.Skill;
        }
    }

    protected override void ExitAttack()
    {
        base.ExitAttack();
        attackCount++;
    }

    IEnumerator JumpAttack()
    {
        // 점프 힘 모으고
        IsNavStop(true);
        anim.CrossFade("Jump Attack", 0.1f, -1, 0);

        yield return new WaitForSeconds(0.45f);

        // 점프하면서 플레이어에게 이동 
        IsNavStop(false);
        nav.SetDestination(_lockTarget.transform.position);
        nav.speed = 20f;

        yield return new WaitForSeconds(1f);

        // 착지 후 데미지
        IsNavStop(true);
        OnAttackDown((int)(_stat.Attack / 2));

        // 원형 공격 범위 생성
        CircleAttackRange();

        yield return new WaitForSeconds(1f);

        // 원형 이펙트 생성
        StartCoroutine(CircleEffect());

        yield return new WaitForSeconds(1f);

        IsNavStop(false);
        nav.speed = 3.5f;
        nav.SetDestination(_lockTarget.transform.position);

        State = Define.State.Moving;
    }

    // 원형 공격 범위 생성
    Vector3 circleAttackRange = new Vector3(3.5f, 0.001f, 3.5f);
    void CircleAttackRange()
    {
        attackRangeObj = Managers.Resource.Instantiate("Object/CircleAttackRange", this.transform);
        attackRangeObj.transform.localPosition = Vector3.up * 0.01f;
        attackRangeObj.transform.localScale = circleAttackRange;
        attackRangeObj.GetOrAddComponent<AttackRange>().SetInfo(_stat, true);
    }

    Vector3 circleEffectScale = Vector3.one * 1.8f;
    IEnumerator CircleEffect()
    {
        // 공격 범위 제거
        Managers.Resource.Destroy(attackRangeObj);

        GameObject effect = Managers.Resource.Instantiate("Effect/Monster/Demon/Demon_Spikes");
        effect.transform.position = transform.position;
        effect.transform.localScale = circleEffectScale;

        yield return new WaitForSeconds(1.5f);

        Managers.Resource.Destroy(effect);
    }

    protected void OnComboAttack()
    {
        base.OnAttackEvent();
    }

    // 넘어지도록 공격
    protected void OnAttackDown(int addDamge = 0)
    {
        distance = TargetDistance(Managers.Game.GetPlayer());

        if (distance <= attackRange)
        {
            Managers.Game._playScene.OnMonsterBar(_stat);

            Managers.Game.GetPlayer().GetComponent<PlayerController>().OnHitDown(_stat, addDamge);
        }
    }

    protected override void UpdateHit() {}
}
