using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossController : MonsterController
{
    int attackCount = 0;    // 공격 횟수 (3번 하면 스킬 진행)

    GameObject attackRangeObj;

    [SerializeField]
    Transform missilePos1;
    [SerializeField]
    Transform missilePos2;

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
                ThinkSkill();
        }
        
        base.UpdateMoving();
    }

    void ThinkSkill()
    {
        State = Define.State.Skill;
        attackCount = 0;

        // Hp가 MaxHp의 반절 보다 높으면 점프 공격만 진행
        if (_stat.Hp > (_stat.MaxHp / 2))
        {
            StopCoroutine(Missile());
            StartCoroutine(Missile());
            return;
        }

        int randomValue = Random.Range(0, 5);
        switch (randomValue)
        {
            case 0:
            case 1:
            case 2:
                StopCoroutine(JumpAttack());
                StartCoroutine(JumpAttack());
                break;
            case 3:
            case 4:
                StopCoroutine(Missile());
                StartCoroutine(Missile());
                break;
        }
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
        nav.speed = 50f;

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

    IEnumerator Missile()
    {
        anim.CrossFade("Roaring", 0.1f, -1, 0);

        for(int i=1; i<=2; i++)
        {
            GameObject missile = Managers.Resource.Instantiate("Effect/Monster/Demon/Missile");
            missile.GetComponent<MissileController>().SetInfo(_stat, 15f);

            if (i == 1)
                missile.transform.position = missilePos1.position;
            else if (i == 2)
                missile.transform.position = missilePos2.position;
        }

        yield return new WaitForSeconds(2f);

        State = Define.State.Moving;
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

    protected override void UpdateHit() {}
}
