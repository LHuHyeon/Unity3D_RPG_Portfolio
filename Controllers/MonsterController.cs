using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterController : BaseController
{
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
            Debug.Log(_lockTarget.name);
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
        {
            BattleClose();
        }
    }

    // TODO : Mixamo에서 애니메이션 찾아 적용
    protected void OnAttackEvent()
    {
        distance = TargetDistance(Managers.Game.GetPlayer());
        if (distance <= attackRange)
        {
            Managers.Game.OnAttacked(_stat);
        }
    }

    protected void ExitAttack()
    {
        State = Define.State.Moving;
    }

    protected override void UpdateDie()
    {

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
