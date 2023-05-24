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

    MonsterStat _stat;
    NavMeshAgent nav;
    
    public GameObject hpBarUI;

    public override void Init()
    {
        WorldObjectType = Define.WorldObject.Monster;

        _stat = GetComponent<MonsterStat>();
        anim = GetComponent<Animator>();
        nav = GetComponent<NavMeshAgent>();

        hpBarUI = Managers.UI.MakeWorldSpaceUI<UI_HpBar>(transform).gameObject;
    }

    protected override void UpdateIdle()
    {
        // Player와 거리 체크
        distance = TargetDistance(Managers.Game.GetPlayer());
        if (distance <= scanRange)
        {
            hpBarUI.SetActive(true);
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
        {
            State = Define.State.Idle;
            BattleClose();
        }
    }

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
            StartCoroutine(DelayDestroy());
    }

    IEnumerator DelayDestroy()
    {
        Destroy(GetComponent<CapsuleCollider>());

        yield return new WaitForSeconds(3f);

        Managers.Game.Despawn(gameObject);
    }

    protected float TargetDistance(GameObject _target)
    {
        return (_target.transform.position - transform.position).magnitude;
    }

    public void BattleClose()
    {
        _lockTarget = null;
        nav.SetDestination(transform.position);
        hpBarUI.SetActive(false);
    }
}
