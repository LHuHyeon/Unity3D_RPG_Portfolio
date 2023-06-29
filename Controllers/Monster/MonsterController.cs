using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterController : BaseController
{
    public Define.MonsterType monsterType = Define.MonsterType.Normal;

    [SerializeField] protected float scanRange;
    [SerializeField] protected float attackRange;
    [SerializeField] protected float spawnRange = 16;

    protected float distance;           // 타겟과의 사이 거리
    protected float rValue=0;           // 준비 시간 랜덤 값

    protected bool isAttack = false;    // 공격 시 true
    protected bool isOverSpawn = false; // 스폰 거리 벗어나면

    protected MonsterStat _stat;
    protected NavMeshAgent nav;

    public Vector3 spawnPos;
    
    public GameObject hpBarUI;

    public override void Init()
    {
        WorldObjectType = Define.WorldObject.Monster;

        _stat = GetComponent<MonsterStat>();
        anim = GetComponent<Animator>();
        nav = GetComponent<NavMeshAgent>();

        hpBarUI = Managers.UI.MakeWorldSpaceUI<UI_HpBar>(transform).gameObject;

        spawnPos = transform.position;
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
        if (isOverSpawn == true)
            return;

        // 스폰 지점에서 일정 거리 벗어나면 스폰지점으로 이동
        float spawnDistance = (spawnPos - transform.position).magnitude;
        if (spawnDistance >= spawnRange)
        {
            StartCoroutine(SpawnMoving());
            return;
        }

        distance = TargetDistance(Managers.Game.GetPlayer());
        Managers.Game._playScene.OnMonsterBar(_stat);
        
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
            StartCoroutine(SpawnMoving());
        }
    }

    // 일정 거리 벗어나면 스폰 지점으로 이동하기
    IEnumerator SpawnMoving()
    {
        isOverSpawn = true;

        BattleClose();

        nav.SetDestination(spawnPos);

        while (true)
        {
            float spawnDistance = (spawnPos - transform.position).magnitude;
            if (spawnDistance <= 0.5f)
                break;

            yield return new WaitForSeconds(0.1f);
        }

        State = Define.State.Idle;
        isOverSpawn = false;
    }

    protected override void UpdateAttack()
    {
        Vector3 dir = Managers.Game.GetPlayer().transform.position - transform.position;
        dir.y = 0;
        transform.rotation = Quaternion.LookRotation(dir);
    }

    // Anim Event
    protected virtual void OnAttackEvent()
    {
        distance = TargetDistance(Managers.Game.GetPlayer());

        if (distance <= attackRange)
        {
            Managers.Game._playScene.OnMonsterBar(_stat);
            Managers.Game.OnAttacked(_stat);
        }
    }

    // Anim Event
    protected virtual void ExitAttack()
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
        GetComponent<CapsuleCollider>().enabled = false;

        yield return new WaitForSeconds(3f);

        State = Define.State.Idle;
        Managers.Game.Despawn(this.gameObject);

        GetComponent<CapsuleCollider>().enabled = true;
        _stat.Hp = _stat.MaxHp;
    }

    public void IsNavStop(bool isTrue)
    {
        if (nav == null)
            nav = GetComponent<NavMeshAgent>();
            
        if (isTrue == true)
        {
            nav.isStopped = true;
            nav.updatePosition = false;
            nav.updateRotation = false;
            nav.velocity = Vector3.zero;
        }
        else
        {
            nav.ResetPath();
            nav.isStopped = false;
            nav.updatePosition = true;
            nav.updateRotation = true;
        }
    }

    protected float TargetDistance(GameObject _target)
    {
        return (_target.transform.position - transform.position).magnitude;
    }

    public void BattleClose()
    {
        _lockTarget = null;
        Managers.Game._playScene.ClostMonsterBar();

        nav.SetDestination(transform.position);
        hpBarUI.SetActive(false);
    }
}
