using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class PlayerController : BaseController
{
    bool _stopAttack = true;   // 공격 가능 여부
    
    bool _isDiveRoll = false;    // 구르기 여부

    // LayerMask 변수
    int _mask = (1 << (int)Define.Layer.Ground) | (1 << (int)Define.Layer.Monster) | (1 << (int)Define.Layer.Npc);

    [SerializeField]
    private List<ObjectData> effects = new List<ObjectData>();
    private GameObject currentEffect;

    public SkillData currentSkill;

    public override void Init()
    {
        WorldObjectType = Define.WorldObject.Player;
        
        anim = GetComponent<Animator>();

        Managers.Input.KeyAction -= OnKeyEvent;
        Managers.Input.KeyAction += OnKeyEvent;
        Managers.Input.MouseAction -= OnMouseEvent;
        Managers.Input.MouseAction += OnMouseEvent;
    }

    protected override void UpdateIdle()
    {
        // TODO : 가만히 있을때의 모션 있으면 사용
        if (_stopAttack == false)
            StopAttack();
    }

    Vector3 dir;
    protected override void UpdateMoving()
    {
        if (_lockTarget != null){
            float distance = (_lockTarget.transform.position - transform.position).magnitude;
            if (distance <= 1f){
                // TODO : npc 상호작용
                State = Define.State.Idle;
                return;
            }
        }

        // 타겟 대상을 클릭했을 때 콜라이더 위쪽을 클릭하게 된다면 그쪽을 바라보고 달리기 때문에 y값을 0으로 준다.
        _destPos.y = 0; 

        // 도착 위치 벡터에서 플레이어 위치 벡터를 뺀다.
        dir = _destPos - transform.position;

        // Vector3.magnitude = 벡터값의 길이
        if (dir.magnitude < 0.1f)
            State = Define.State.Idle;
        else
        {
            // 건물을 클릭하여 이동하면 건물 앞에 멈추기 (1.0f 거리에서 멈추기)
            Debug.DrawRay(transform.position + (Vector3.up * 0.5f), dir.normalized, Color.red);
            if (Physics.Raycast(transform.position + (Vector3.up * 0.5f), dir, 1.0f, LayerMask.GetMask("Block"))){
                if (Input.GetMouseButton(0) == false)
                    State = Define.State.Idle;
                return;
            }

            float moveDist = Mathf.Clamp(Managers.Game.MoveSpeed * Time.deltaTime, 0, dir.magnitude);
            
            transform.position += dir.normalized * moveDist;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 20f * Time.deltaTime);
        }
    }

    protected override void UpdateDiveRoll()
    {
        StopAttack();

        _destPos = GetMousePoint();

        float moveDist = Mathf.Clamp(Managers.Game.MoveSpeed * Time.deltaTime, 0, dir.magnitude);

        transform.position += dir.normalized * moveDist;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 20f * Time.deltaTime);
    }

    float _attackCloseTime = 0;
    protected override void UpdateAttack()
    {
        _attackCloseTime += Time.deltaTime;

        if (anim.GetCurrentAnimatorStateInfo(0).IsTag("Attack") &&
            _attackCloseTime > 0.94f &&
            _onComboAttack == false)
        {
            StopAttack();
            State = Define.State.Idle;
            return;
        }

        dir = _destPos - transform.position;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 20f * Time.deltaTime);
    }

    protected override void UpdateSkill()
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsTag("Skill") &&
            anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.95f)
        {
            EffectClose();
            State = Define.State.Idle;
        }
    }

    // 마우스 클릭
    void OnMouseEvent(Define.MouseEvent evt)
    {
        switch(State)
        {
            case Define.State.Moving:
                GetMouseEvent(evt);
                break;
            case Define.State.Idle:
                GetMouseEvent(evt);
                break;
            case Define.State.Attack:
                GetMouseEvent(evt);
                break;
        }
    }

    float minDistance = 0.3f;
    void GetMouseEvent(Define.MouseEvent evt)
    {
        // 메인 카메라에서 마우스가 가르키는 위치의 ray를 저장
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        bool raycastHit = Physics.Raycast(ray, out hit, 150f, _mask);
        
        // 자신 캐릭터 클릭 시 진행 X
        float distance = (hit.point - transform.position).magnitude;
        if (distance <= minDistance)
            return;
        
        switch (evt)
        {
            // 마우스를 클릭했을 때 [ 클릭한 위치로 이동 ]
            case Define.MouseEvent.RightDown:
                {
                    _destPos = hit.point;   // 해당 좌표 저장
                    if (raycastHit && _stopAttack)
                    {
                        State = Define.State.Moving;

                        if (hit.collider.gameObject.layer == (int)Define.Layer.Npc)
                            _lockTarget = hit.collider.gameObject;
                        else
                            _lockTarget = null;
                    }
                }
                break;
            // 마우스를 클릭 중일 때
            case Define.MouseEvent.RightPress:
                {
                    if (_stopAttack)
                    {
                        if (State == Define.State.Idle)
                            State = Define.State.Moving;

                        if (_lockTarget != null)
                            _destPos = _lockTarget.transform.position;
                        else if (raycastHit)
                            _destPos = hit.point;
                    }
                }
                break;
            // 왼쪽 클릭 시 공격
            case Define.MouseEvent.LeftDown:
                {
                    _stopAttack = false;
                    _destPos = hit.point;
                    _destPos.y = 0;
                    OnAttack();
                }
                break;
            // 왼쪽 누르는 중이면 다음 공격 진행
            case Define.MouseEvent.LeftPress:
                {
                    _destPos = hit.point;
                    _destPos.y = 0;
                    OnAttack();
                }
                break;
        }
    }

    bool _onAttack = false;
    bool _onComboAttack = false;
    int attackClipNumber = 0;
    string[] _attackClipList = new string[] {"ATTACK1", "ATTACK2"};
    void OnAttack()
    {
        // 콤보 체크
        if (_onAttack &&
            anim.GetCurrentAnimatorStateInfo(0).IsTag("Attack") &&
            anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.3f)
        {
            _onComboAttack = true;
        }

        // 공격!
        if (!_onAttack)
        {
            anim.CrossFade(_attackClipList[attackClipNumber], 0.1f, -1, 0);
            State = Define.State.Attack;

            _onAttack = true;
        }
    }

    // [ Anim Event ]
    public void ExitAttack()
    {
        // boxColider.SetActive(false);
        if (_onComboAttack == true)
        {
            if (attackClipNumber == 1)
                attackClipNumber = 0;
            else if (attackClipNumber == 0)
                attackClipNumber = 1;

            anim.CrossFade(_attackClipList[attackClipNumber], 0.1f, -1, 0);
            _onComboAttack = false;
        }
    }

    void StopAttack()
    {
        _onAttack = false;
        _onComboAttack = false;
        _stopAttack = true;
        _attackCloseTime = 0;
        attackClipNumber = 0;
    }

    // 키보드 클릭
    void OnKeyEvent()
    {
        if (_isDiveRoll == false)
        {
            GetDiveRoll();
            GetSkill();
        }
    }

    // [ Anim Event ]
    public void EventDiveRoll()
    {
        _isDiveRoll = false;
        Managers.Game.MoveSpeed = 5;
        State = Define.State.Idle;
    }

    void GetDiveRoll()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _isDiveRoll = true;
            State = Define.State.DiveRoll;

            _destPos = GetMousePoint();
            dir = _destPos - transform.position;
            
            Managers.Game.MoveSpeed = 8;

            EffectClose();
        }
    }

    // 스킬 사용
    void GetSkill()
    {
        if (State == Define.State.Skill)
            return;

        // 마우스 방향으로 회전
        _destPos = GetMousePoint();
        dir = _destPos - transform.position;
        transform.rotation = Quaternion.LookRotation(dir);

        // TEST CODE
        if (Input.GetKeyDown(KeyCode.Q))
            OnSkill(Managers.Game.GetSkill(101));

        // 스킬 진행 (스킬 ui 완성되면 사용)
        // if (Input.GetKeyDown(KeyCode.Q))
        //     OnSkill(Managers.Game.GetSkill(Define.KeySkill.Q));
        // else if (Input.GetKeyDown(KeyCode.W))
        //     OnSkill(Managers.Game.GetSkill(Define.KeySkill.W));
        // else if (Input.GetKeyDown(KeyCode.E))
        //     OnSkill(Managers.Game.GetSkill(Define.KeySkill.E));
        // else if (Input.GetKeyDown(KeyCode.A))
        //     OnSkill(Managers.Game.GetSkill(Define.KeySkill.A));
        // else if (Input.GetKeyDown(KeyCode.S))
        //     OnSkill(Managers.Game.GetSkill(Define.KeySkill.S));
        // else if (Input.GetKeyDown(KeyCode.D))
        //     OnSkill(Managers.Game.GetSkill(Define.KeySkill.D));
        // else if (Input.GetKeyDown(KeyCode.R))
        //     OnSkill(Managers.Game.GetSkill(Define.KeySkill.R));
    }

    // 스킬 진행
    void OnSkill(SkillData skill)
    {
        currentSkill = skill;
        if (currentSkill == null)
        {
            Debug.Log("등록된 스킬이 없습니다!");
            return;
        }

        foreach(ObjectData value in effects)
        {
            if (currentSkill.skillId == value.id)
            {
                currentEffect = value.gameObject;
                break;
            }
        }

        State = Define.State.Skill;
        anim.CrossFade("SKILL"+currentSkill.skillId, 0.1f, -1, 0);
        currentEffect.SetActive(true);
    }

    // 마우스 Ray
    public Vector3 GetMousePoint()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(ray, out hit, 150f, _mask);
        
        Vector3 hitPoint = hit.point;
        hitPoint.y = 0;
        return hitPoint;
    }

    public void EffectClose()
    {
        if (currentEffect != null)
            currentEffect.SetActive(false);
    }
}
