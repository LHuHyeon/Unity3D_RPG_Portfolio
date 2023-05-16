using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterController : BaseController
{
    public int id;

    [SerializeField] private float scanRange;
    [SerializeField] private float attackRange;

    protected float distance;           // 타겟과의 사이 거리
    protected float rValue=0;           // 준비 시간 랜덤 값

    protected bool isAttack = false;    // 공격 시 true

    Stat _stat;
    NavMeshAgent nav;
    
    protected GameObject hpBarUI;

    public override void Init()
    {
        WorldObjectType = Define.WorldObject.Monster;

        _stat = GetComponent<Stat>();
        anim = GetComponent<Animator>();
        nav = GetComponent<NavMeshAgent>();
    }

    protected override void UpdateIdle()
    {
        // Player와 거리 체크
        distance = TargetDistance(Managers.Game.GetPlayer());
        if (distance <= scanRange)
        {
            // hpBarUI.SetActive(true);
            _lockTarget = Managers.Game.GetPlayer();
            State = Define.State.Moving;
        }
    }

    protected override void UpdateMoving()
    {
        distance = TargetDistance(Managers.Game.GetPlayer());
        if (distance <= scanRange)
        {
            nav.SetDestination(_lockTarget.transform.position);

            if (distance <= attackRange)
            {
                nav.SetDestination(transform.position);
                State = Define.State.Attack;
            }
        }
        else
            BattleClose();
    }

    // protected override void UpdateAttack()
    // {
    //     if (anim.GetCurrentAnimatorStateInfo(0).IsTag("Attack") &&
    //         anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 7f)
    //     {
            
    //     }
    // }

    // Anim Event
    protected void OnAttackEvent()
    {
        distance = TargetDistance(Managers.Game.GetPlayer());

        if (distance <= attackRange)
            Managers.Game.OnAttacked(_stat);
    }

    // Anim Event
    protected void ExitAttack()
    {
        State = Define.State.Moving;
    }

    protected override void UpdateHit()
    {
        nav.SetDestination(transform.position);
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("HIT") &&
            anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.9f)
        {
            State = Define.State.Moving;
        }
    }

    protected override void UpdateDie()
    {
        nav.SetDestination(transform.position);

        if (GetComponent<CapsuleCollider>() != null)
            Destroy(GetComponent<CapsuleCollider>());

        if (anim.GetCurrentAnimatorStateInfo(0).IsName("DIE") &&
            anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.9f)
        {
            Managers.Game.Despawn(gameObject);
        }
    }

    protected float TargetDistance(GameObject _target)
    {
        return (_target.transform.position - transform.position).magnitude;
    }

    public void BattleClose()
    {
        _lockTarget = null;
        nav.SetDestination(transform.position);
        // hpBarUI.SetActive(false);
        State = Define.State.Idle;
    }
}