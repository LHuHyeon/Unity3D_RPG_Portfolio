using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseController : MonoBehaviour
{
    protected Animator anim;
    protected RaycastHit hit;

    [SerializeField]
    protected GameObject _lockTarget;   // 마우스로 타겟한 오브젝트 담는 변수
    
    [SerializeField]
    protected Vector3 _destPos;         // 도착 좌표

    [SerializeField]
    protected Define.State _state = Define.State.Idle; // 상태 변수

    [SerializeField]
    public Define.WorldObject WorldObjectType { get; protected set; } = Define.WorldObject.Unknown;

    int number = 1;

    // 캐릭터 상태에 따라 애니메이션이 작동하는 _state의 프로퍼티
    public virtual Define.State State
    {
        get { return _state; }
        set {
            _state = value;

            // anim.CrossFade(재생 클립 이름, 바뀌는데에 지연 시간, 레이어, 재생 시작 시점)
            switch (_state)
            {
                case Define.State.Moving:
                    anim.CrossFade("RUN", 0.1f);
                    break;
                case Define.State.Idle:
                    anim.CrossFade("WAIT", 0.4f);
                    break;
                case Define.State.DiveRoll:
                    anim.CrossFade("DIVEROLL", 0.1f, -1, 0);
                    break;
                case Define.State.Attack:
                    if (WorldObjectType == Define.WorldObject.Monster)
                    {
                        anim.CrossFade("ATTACK"+number, 0.1f, -1, 0);

                        if (number == 1)
                            number = 2;
                        else if (number == 2)
                            number = 1;
                    }
                    break;
                case Define.State.Hit:
                    {
                        anim.CrossFade("HIT", 0.1f, -1, 0);
                    }
                    break;
                case Define.State.Die:
                    {
                        anim.CrossFade("DIE", 0.1f, -1, 0);
                    }
                    break;
            }
        }
    }

    void Start()
    {
        Init();
    }

    // Playe, NPC 전용 ( 키 입력이 필요한 경우 )
    void Update()
    {
        if (WorldObjectType == Define.WorldObject.Monster)
            return;

        switch (State)
        {
            case Define.State.Moving:    // 움직임
                UpdateMoving();
                break;
            case Define.State.DiveRoll:  // 구르기
                UpdateDiveRoll();       
                break;
            case Define.State.Idle:      // 가만히 있기
                UpdateIdle();
                break;
            case Define.State.Attack:     // 일반 공격
                UpdateAttack();
                break;
            case Define.State.Skill:     // 스킬
                UpdateSkill();
                break;
            case Define.State.Hit:       // 피격
                UpdateHit();
                break;
            case Define.State.Die:       // 죽음
                UpdateDie();
                break;
        }
    }

    // Monster 전용
    void FixedUpdate()
    {
        if (WorldObjectType != Define.WorldObject.Monster)
            return;

        switch (State)
        {
            case Define.State.Moving:    // 움직임
                UpdateMoving();
                break;
            case Define.State.Idle:      // 가만히 있기
                UpdateIdle();
                break;
            case Define.State.Attack:     // 일반 공격
                UpdateAttack();
                break;
            case Define.State.Skill:     // 스킬
                UpdateSkill();
                break;
            case Define.State.Hit:       // 피격
                UpdateHit();
                break;
            case Define.State.Die:       // 죽음
                UpdateDie();
                break;
        }
    }

    public abstract void Init();

    protected virtual void UpdateMoving() {}
    protected virtual void UpdateDiveRoll() {}
    protected virtual void UpdateIdle() {}
    protected virtual void UpdateAttack() {}
    protected virtual void UpdateSkill() {}
    protected virtual void UpdateHit() {}
    protected virtual void UpdateDie() {}
}
