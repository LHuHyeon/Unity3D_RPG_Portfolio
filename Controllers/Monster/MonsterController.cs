using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/*
 * File :   MonsterController.cs
 * Desc :   몬스터의 모든 상태를 관리
 *
 & Functions
 &  [Public]
 &  : Init()        - 초기 설정
 &  : BattleClose() - 전투 종료
 &
 &  [Protected]
 &  : IdleTargetDetection()     - Idle 상태에서 타겟 감지
 &  : UpdateIdle()              - 멈춤일 때   Update  (플레이어 감지)
 &  : UpdateMoving()            - 움직일 때   Update  (플레이어 추격)
 &  : UpdateAttack()            - 공격할 때   Update
 &  : UpdateHit()               - 피격받을 때 Update
 &  : UpdateDie()               - 죽었을 때   Update
 &  : OnAttackEvent()           - 공격 시작 (Animation Event)
 &  : ExitAttack()              - 공격 끝   (Animation Event)
 &  : TargetDistance()          - 타겟 거리값
 &
 &  [Private]
 &  : SpawnMoving()     - 스폰 지점 이동 코루틴
 &  : DelayDestroy()    - 딜레이 삭제 코루틴
 *
 */

public class MonsterController : BaseController
{
    public Define.MonsterType   monsterType;            // 몬스터 타입
    public Vector3              spawnPos;               // 스폰 위치
    public GameObject           hpBarUI;                // 체력바 UI

    protected MonsterStat       _stat;                  // 몬스터 스탯
    protected NavMeshAgent      nav;

    protected float             distance;               // 타겟과의 사이 거리
    protected bool              isOverSpawn = false;    // 스폰거리에서 벗어났는지 체크

    [SerializeField] protected float scanRange;         // 플레이어 감지 거리
    [SerializeField] protected float attackRange;       // 공격 사거리
    [SerializeField] protected float spawnRange = 16;   // 스폰 사거리 Max 거리

    // 초기 설정
    public override void Init()
    {
        monsterType = Define.MonsterType.Normal;
        WorldObjectType = Define.WorldObject.Monster;

        _stat = GetComponent<MonsterStat>();
        anim = GetComponent<Animator>();
        nav = GetComponent<NavMeshAgent>();

        // 체력바 생성
        hpBarUI = Managers.UI.MakeWorldSpaceUI<UI_HpBar>(transform).gameObject;

        // 스폰 위치 설정
        spawnPos = transform.position;
    }

    // Idle 상태에서 타겟 감지 시
    protected virtual void IdleTargetDetection()
    {
        hpBarUI.SetActive(true);                    // 체력바 활성화
        _lockTarget = Managers.Game.GetPlayer();    // 타겟 설정

        State = Define.State.Moving;
    }

    // Idle Update
    protected override void UpdateIdle()
    {
        // 플레이어 사망 시 작동 X
        if (Managers.Game.GetPlayer().GetComponent<PlayerController>().State == Define.State.Die)
            return;

        // 플레이어와 거리 체크
        distance = TargetDistance(Managers.Game.GetPlayer());
        if (distance <= scanRange)
            IdleTargetDetection();
    }

    // Moving Update
    protected override void UpdateMoving()
    {
        // 스폰거리 초과 체크
        if (isOverSpawn == true)
            return;

        // 플레이어가 죽었거나, 타겟이 Null이면
        if (Managers.Game.GetPlayer().GetComponent<PlayerController>().State == Define.State.Die ||
            _lockTarget.IsNull() == true)
        {
            StartCoroutine(SpawnMoving());  // 스폰 지점으로 이동
            return;
        }

        // 스폰 지점에서 일정 거리 벗어나면 스폰지점으로 이동
        float spawnDistance = (spawnPos - transform.position).magnitude;
        if (spawnDistance >= spawnRange)
        {
            StartCoroutine(SpawnMoving());  // 스폰 지점으로 이동
            return;
        }

        distance = TargetDistance(_lockTarget);         // 타겟 거리값
        Managers.Game._playScene.OnMonsterBar(_stat);   // Scene UI 몬스터 정보 활성화

        // 타겟과의 거리가 일정 범위 벗어나면
        if (distance > scanRange)
        {
            StartCoroutine(SpawnMoving());  // 스폰 지점으로 이동
            return;
        }
        
        // nav 도착좌표 설정
        nav.SetDestination(_lockTarget.transform.position);

        // 타겟이 공격사거리안에 들어오면
        if (distance <= attackRange)
        {
            // 멈추고 공격 시작
            nav.SetDestination(transform.position);
            State = Define.State.Attack;
        }
    }

    // Attack Update
    protected override void UpdateAttack()
    {
        // 플레이어가 죽었다면
        if (Managers.Game.GetPlayer().GetComponent<PlayerController>().State == Define.State.Die)
        {
            State = Define.State.Moving;
            return;
        }
        
        // 회전값 설정
        Vector3 dir = Managers.Game.GetPlayer().transform.position - transform.position;
        dir.y = 0;
        transform.rotation = Quaternion.LookRotation(dir);
    }

    // Hit Update
    protected override void UpdateHit()
    {
        // 멈추기
        nav.SetDestination(transform.position);

        // 피격 애니메이션 시간 체크
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("HIT") &&
            anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.9f)
        {
            State = Define.State.Moving;
        }
    }

    // Die Update
    protected override void UpdateDie()
    {
        // 멈추기
        nav.SetDestination(transform.position);

        // 콜라이더가 Null이 아니라면 삭제 진행
        if (GetComponent<CapsuleCollider>().IsNull() == false)
            StartCoroutine(DelayDestroy());
    }

    // 공격할 때 (Animation Event)
    protected virtual void OnAttackEvent()
    {
        // 타겟 거리값
        distance = TargetDistance(Managers.Game.GetPlayer());

        // 공격 사거리에 있으면
        if (distance <= attackRange)
        {
            // Scene UI 몬스터 정보 활성화
            Managers.Game._playScene.OnMonsterBar(_stat);

            // 플레이어 데미지 반영
            Managers.Game.OnAttacked(_stat);
        }
    }

    // 공격이 끝날때 (Animation Event)
    protected virtual void ExitAttack()
    {
        State = Define.State.Moving;
    }

    // 타겟 거리값
    protected float TargetDistance(GameObject _target)
    {
        if (_target.IsNull() == true) return 0;
        return (_target.transform.position - transform.position).magnitude;
    }

    // 스폰 지점 이동 코루틴
    private IEnumerator SpawnMoving()
    {
        isOverSpawn = true;

        BattleClose();  // 전투 종료

        nav.SetDestination(spawnPos);   // 스폰 위치로

        // 스폰과 가까워지면 멈추기
        while (true)
        {
            float spawnDistance = (spawnPos - transform.position).magnitude;
            if (spawnDistance <= 0.7f)
                break;

            yield return null;
        }

        State = Define.State.Idle;

        isOverSpawn = false;
    }

    // 삭제 딜레이 코루틴
    private IEnumerator DelayDestroy()
    {
        // 콜라이더 비활성화 ( 플레이어 감지 때문 )
        GetComponent<CapsuleCollider>().enabled = false;

        // Scene UI 몬스터 정보 삭제
        Managers.Game._playScene.CloseMonsterBar();

        yield return new WaitForSeconds(3f);

        // 몬스터 삭제 ( Pool )
        State = Define.State.Idle;
        Managers.Game.Despawn(this.gameObject);

        // 콜라이더 활성화
        GetComponent<CapsuleCollider>().enabled = true;

        // 체력 복구
        _stat.Hp = _stat.MaxHp;
    }

    // 전투 종료 
    public void BattleClose()
    {
        _lockTarget = null;
        Managers.Game._playScene.CloseMonsterBar();

        nav.SetDestination(transform.position);
        hpBarUI.SetActive(false);
    }
}
