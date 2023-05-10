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

    // 캐릭터 상태에 따라 애니메이션이 작동하는 _state의 프로퍼티
    public virtual Define.State State
    {
        get { return _state; }
        set {
            _state = value;

            if (WorldObjectType != Define.WorldObject.Player)
            {
                switch (_state){
                    case Define.State.Moving:
                        anim.CrossFade("RUN", 0.1f);    // CrossFade는 애니메이션의 부드러움, 반복도 등 설정이 가능하다.
                        break;
                    case Define.State.Idle:
                        anim.CrossFade("WAIT", 0.1f);
                        break;
                    case Define.State.Attack:
                        anim.CrossFade("ATTACK", 0.1f, -1, 0);
                        break;
                    case Define.State.Die:
                        break;
                }
            }
        }
    }

    void Start()
    {
        Init();
    }

    void Update()
    {
        // State 패턴
        switch (State){
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
            case Define.State.Die:       // 죽음
                UpdateDie();
                break;
        }
    }

    public abstract void Init();

    protected virtual void UpdateMoving() {}
    protected virtual void UpdateIdle() {}
    protected virtual void UpdateAttack() {}
    protected virtual void UpdateSkill() {}
    protected virtual void UpdateDie() {}
}
