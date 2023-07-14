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

    Portal exitPortal;

    public override void Init()
    {
        base.Init();

        monsterType = Define.MonsterType.Boss;

        exitPortal = GameObject.FindObjectOfType<Portal>();
        if (exitPortal != null)
            exitPortal.gameObject.SetActive(false);
    }

    protected override void UpdateMoving()
    {
        // 공격 횟수가 3회 도달하면 점프 공격 진행
        // 거리 상관없이 사용해야 되므로 Moving 상태에서 진행
        if (attackCount >= 3)
            ThinkSkill();
        
        base.UpdateMoving();
    }

    void ThinkSkill()
    {
        if (_lockTarget == null)
            return;

        State = Define.State.Skill;
        attackCount = 0;

        int randomValue = Random.Range(0, 9);
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
            case 5:
                StopCoroutine(Missile());
                StartCoroutine(Missile());
                break;
            case 6:
            case 7:
            case 8:
                StopCoroutine(AOEJumpAttackSkill());
                StartCoroutine(AOEJumpAttackSkill());
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
            attackCount++;
        }
    }

    protected override void ExitAttack()
    {
        base.ExitAttack();
        attackCount++;
    }

    // 점프하여 플레이어에게 착지한다.
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
        CircleAttackRange(new Vector3(3.5f, 0.001f, 3.5f));

        yield return new WaitForSeconds(1f);

        // 원형 이펙트 생성
        StartCoroutine(CircleEffect());

        yield return new WaitForSeconds(1f);

        IsNavStop(false);
        nav.speed = 3.5f;
        nav.SetDestination(Managers.Game.GetPlayer().transform.position);

        State = Define.State.Moving;
    }

    // 두개의 작은 미사일을 쏜다.
    IEnumerator Missile()
    {
        IsNavStop(true);
        anim.CrossFade("Roaring", 0.1f, -1, 0);

        yield return new WaitForSeconds(0.5f);

        for(int i=1; i<=2; i++)
        {
            GameObject missile = Managers.Resource.Instantiate("Effect/Monster/Demon/Missile");
            missile.GetComponent<MissileController>().SetInfo(_stat, 8f);

            if (i == 1)
                missile.transform.position = missilePos1.position;
            else if (i == 2)
                missile.transform.position = missilePos2.position;
        }

        yield return new WaitForSeconds(0.5f);

        // 미사일 쏘고 잠시 기다리기
        anim.CrossFade("WAIT", 0.4f);

        yield return new WaitForSeconds(4f);

        IsNavStop(false);
        nav.SetDestination(Managers.Game.GetPlayer().transform.position);
        State = Define.State.Moving;
    }

    // 제자리 점프 후 착지한다. 착지 후 지속적으로 데미지를 주는 원을 생성한다.
    IEnumerator AOEJumpAttackSkill()
    {
        // 가만히 점프!
        IsNavStop(true);
        anim.CrossFade("Idle Jump", 0.1f, -1, 0);

        yield return new WaitForSeconds(2.2f);

        // 착지 후
        // 근처에 플레이어가 있으면 넘어트리며 공격
        OnAttackDown((int)(_stat.Attack * 0.5f));

        // 원형 공격 범위 생성
        CircleAttackRange(new Vector3(5f, 0.001f, 5f));

        yield return new WaitForSeconds(1f);

        // 원형 공격 범위 제거
        Managers.Resource.Destroy(attackRangeObj);

        // 범위 지속 공격 생성
        GameObject attackAOE = Managers.Resource.Instantiate("Effect/Monster/Demon/Meteors_AOE");
        attackAOE.GetOrAddComponent<AOEController>().damage = (int)(_stat.Attack * 0.5f);
        attackAOE.transform.position = transform.position + (Vector3.up * 0.1f);
        attackAOE.transform.localScale = Vector3.one * 1.5f;

        anim.CrossFade("WAIT", 0.4f);

        yield return new WaitForSeconds(7f);

        // 범위 지속 공격 제거
        Managers.Resource.Destroy(attackAOE);

        // 움직이기
        IsNavStop(false);
        nav.SetDestination(Managers.Game.GetPlayer().transform.position);
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
    void CircleAttackRange(Vector3 circleAttackRange)
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

    protected override void UpdateDie()
    {
        base.UpdateDie();

        if (exitPortal == null)
            return;

        // 나가는 포탈 생성
        if (exitPortal.gameObject.activeSelf == false)
        {
            string message = $"<size=170%>Clear!!</size> \n<color=yellow>Gold: 100</color> <color=green>Exp: 200</color>";
            Managers.UI.ShowPopupUI<UI_GuidePopup>().SetInfo(message, new Color(1f, 0.5f, 0f));
            
            exitPortal.gameObject.SetActive(true);
        }
    }
}
