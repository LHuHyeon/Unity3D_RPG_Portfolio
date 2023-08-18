using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DarkKnightController : MonsterController
{
    string[] skills = new string[]{"SKILL1", "SKILL2"};
    string[] meleeAttacks = new string[]{"ATTACK1", "ATTACK2"};
    string[] rangedAttacks = new string[]{"ATTACK3", "ATTACK4", "ATTACK5"};

    int attackCount = 0;    // 공격 횟수 ( 스킬을 사용하기 위함. )
    int onSkillCount = 3;   // 스킬 시작 횟수 
    
    bool isRangedAttack = false;    // 원거리 공격 체크
    bool isSkill = false;           // 다음 스킬 공격 확인

    [SerializeField]
    float rangedAttackRange = 5f;   // 원거리 수치

    [SerializeField]
    AttackCollider skillCollider;   // 스킬 사용 접촉 확인

    [SerializeField]
    AttackCollider attackCollider;  // 일반 공격 사용 접촉 확인

    [SerializeField]
    EffectParticle particleCollider;// 파티클 접촉 확인

    [SerializeField]
    GameObject swingTrail;          // 검기 Trail

    [SerializeField]
    Transform attackRangeObj;       // 공격 예상 범위 오브젝트

    Portal exitPortal;

    public override void Init()
    {
        base.Init();

        particleCollider.SetInfo(()=>{_lockTarget.GetComponent<PlayerController>().OnHitDown(_stat, (int)(_stat.Attack * 0.8f));});

        skillCollider.damage = (int)(_stat.Attack * 1.5f);
        attackCollider.damage = _stat.Attack;

        exitPortal = GameObject.FindObjectOfType<Portal>();
        if (exitPortal.IsNull() == false)
            exitPortal.gameObject.SetActive(false);
    }

    protected override void UpdateIdle()
    {
        if (Managers.Game.GetPlayer().GetComponent<PlayerController>().State == Define.State.Die)
            return;

        distance = TargetDistance(Managers.Game.GetPlayer());
        if (distance <= scanRange)
        {
            hpBarUI.SetActive(true);
            _lockTarget = Managers.Game.GetPlayer();

            isRangedAttack = Random.Range(0, 2) == 0;   // 50% 확률로 원거리, 근거리 공격 결정

            State = Define.State.Moving;
        }
    }

    protected override void UpdateMoving()
    {
        Managers.Game._playScene.OnMonsterBar(_stat);

        distance = TargetDistance(_lockTarget);
        
        nav.SetDestination(_lockTarget.transform.position);
        OnRotation();

        AttackCheck();
    }

    protected override void UpdateAttack()
    {
        OnAnimationMove();
    }

    protected override void UpdateSkill()
    {
        OnAnimationMove();
    }

    protected override void UpdateDie()
    {
        base.UpdateDie();

        if (exitPortal.IsNull() == true)
            return;

        // 나가는 포탈 생성
        if (exitPortal.gameObject.activeSelf == false)
        {
            string message = $"<size=170%>Clear!!</size> \n<color=yellow>Gold: 100</color> <color=green>Exp: 200</color>";
            Managers.UI.MakeSubItem<UI_Guide>().SetInfo(message, new Color(1f, 0.5f, 0f));
            
            exitPortal.gameObject.SetActive(true);
        }
    }

    // 다음 공격 확인
    private void AttackCheck()
    {
        // 스킬 공격이 가능하다면
        if (isSkill == true)
        {
            if (distance <= rangedAttackRange+1)
                OnSkill(skills[Random.Range(0, 2)]);

            return;
        }

        // 원거리에서 공격 시작할지
        if (isRangedAttack == true)
        {
            if (distance <= rangedAttackRange)
                OnAttack(rangedAttacks[Random.Range(0, 3)]);
        }
        else
        {
            if (distance <= attackRange)
                OnAttack(meleeAttacks[Random.Range(0, 2)]);
        }
    }

    // 공격 시작 시
    private void OnAttack(string attackName)
    {
        SetAnimation(attackName);
        State = Define.State.Attack;
    }

    // 스킬 시작 시
    private void OnSkill(string skillName)
    {
        SetAnimation(skillName);

        if (skillName == skills[0])
        {
            StopCoroutine(Skill01_Prick());
            StartCoroutine(Skill01_Prick());
        }
        else if (skillName == skills[1])
        {
            StopCoroutine(Skill02_WeaponDown());
            StartCoroutine(Skill02_WeaponDown());
        }

        isSkill = false;
        State = Define.State.Skill;
    }

    // 찌르기 스킬
    IEnumerator Skill01_Prick()
    {
        // 공격 예상 범위 사이즈 설정
        attackRangeObj.localPosition = new Vector3(0, 0, 2.33f);
        attackRangeObj.localScale = new Vector3(1, 0.00055f, 4.66f);

        yield return new WaitForSeconds(0.4f);  // 찌르기 준비

        skillCollider.IsCollider(true);

        yield return new WaitForSeconds(0.8f);  // 찌르기

        skillCollider.IsCollider(false);

        yield return new WaitForSeconds(1.2f);

        State = Define.State.Idle;
    }

    // 내려찍기 스킬
    IEnumerator Skill02_WeaponDown()
    {
        // 공격 예상 범위 사이즈 설정
        attackRangeObj.localPosition = new Vector3(0, 0, 4.5f);
        attackRangeObj.localScale = new Vector3(1, 0.00055f, 9f);
        
        // 0.9초 동안 플레이어를 바라본 후 공격
        float currentTime = 0f;
        while (true)
        {
            if (currentTime >= 0.9f)
                break;

            currentTime += Time.deltaTime;
            transform.rotation = Quaternion.LookRotation(_lockTarget.transform.position - transform.position);

            yield return null;
        }

        yield return new WaitForSeconds(2f);

        State = Define.State.Idle;
    }

    // 애니메이션 및 방향 설정
    private void SetAnimation(string animName)
    {
        Vector3 distance = _lockTarget.transform.position - transform.position;

        nav.SetDestination(transform.position);
        transform.rotation = Quaternion.LookRotation(distance);

        anim.CrossFade(animName, 0.1f, -1, 0);
    }

    // 네비게이션 자연스러운 즉각 회전 (떨림 완화)
    private void OnRotation()
    {
        Vector2 forward = new Vector2(transform.position.z, transform.position.x);
        Vector2 steeringTarget = new Vector2(nav.steeringTarget.z, nav.steeringTarget.x);
        
        //방향을 구한 뒤, 역함수로 각을 구한다.
        Vector2 dir = steeringTarget - forward;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        
        //방향 적용
        transform.eulerAngles = Vector3.up * angle;
    }

    // 애니메이션 움직임으로 설정
    private void OnAnimationMove()
    {
        Vector3 rootPosition = anim.targetPosition;
        rootPosition.y = nav.nextPosition.y;
        transform.position = rootPosition;
        nav.SetDestination(rootPosition);
    }

    // 공격할 때 (Animation Event)
    Coroutine weaponDisableCo;
    protected override void OnAttackEvent()
    {
        attackCollider.IsCollider(true);

        if (weaponDisableCo.IsNull() == false) StopCoroutine(WeaponDisable());
        weaponDisableCo = StartCoroutine(WeaponDisable());
    }

    // 공격이 끝날때 (Animation Event)
    protected override void ExitAttack()
    {
        // 2~3번 일반 공격 시 다음 공격은 스킬 진행
        if (++attackCount >= Random.Range(2, onSkillCount+1))
        {
            attackCount = 0;
            isSkill = true;
        }

        attackCollider.IsCollider(false);
        State = Define.State.Idle;
    }

    IEnumerator WeaponDisable()
    {
        yield return new WaitForSeconds(0.15f);

        attackCollider.IsCollider(false);
    }

    // 검기 Animation Event 사용 중
    void OnTrail() { swingTrail.SetActive(true); }
    void OffTrail() { swingTrail.SetActive(false); }

    // 보스는 변칙적인 공격이 있기 때문에 사용 x (이대로만 두기)
    protected override void AnimAttack() {}
}
