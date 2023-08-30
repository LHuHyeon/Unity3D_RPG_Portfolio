using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using UnityEngine;

/*
[ 플레이어 컨트롤러 스크립트 ]
1. SetInfo : 장비 파츠(SkinnedMeshRenderer) 찾아서 저장
2. State : Idle, Moving, DiveRoll(구르기), Attack, Hit
3. 기능 : 마우스 입력, 키 입력, 연속 공격, 스킬, 아이템 줍기
*/

public class PlayerController : BaseController
{
    // 모든 장비 오브젝트 저장
    public Dictionary<int, List<GameObject>> charEquipment;

    public GameObject   clickMoveEffect;    // 클릭 이동 파티클 Prefab
    public GameObject   waeponObjList;      // 무기 Prefab List
    public GameObject   currentEffect;      // 현재 이펙트
    public SkillData    currentSkill;       // 현재 스킬

    // Click LayerMask
    private int         _mask = (1 << (int)Define.Layer.Ground) | (1 << (int)Define.Layer.Monster) | (1 << (int)Define.Layer.Npc);

    private bool        _stopAttack = true;     // 공격 가능 여부
    private bool        _isDiveRoll = false;    // 구르기 여부
    private bool        _isDown     = false;    // 넘어진 상태 여부
    
    private float       currentDiveTime = 0f;   // 현재 구르는 시간

    private Vector3     dir;

    [SerializeField]
    private float       _attackCloseTime = 0;   // 공격 취소 시간

    [SerializeField]
    private GameObject  rootBone;               // SkinnedMeshRenderer 대표 뼈대

    [SerializeField]
    private List<EffectData> effects;           // 이펙트 관리 변수

    public override void Init()
    {
        WorldObjectType = Define.WorldObject.Player;
        State = Define.State.Idle;
        
        charEquipment = new Dictionary<int, List<GameObject>>();
        currentEffect = null;

        anim = GetComponent<Animator>();

        // 입력 매니저에서 관리
        Managers.Input.KeyAction -= OnKeyEvent;
        Managers.Input.KeyAction += OnKeyEvent;
        Managers.Input.MouseAction -= OnMouseEvent;
        Managers.Input.MouseAction += OnMouseEvent;

        SetPart();
    }
    
    // 피격 받기 (넘어지는 공격에 호출됨)
    private Coroutine co_HitDown;
    public void OnHitDown(MonsterStat attacker, int addDamge = 0)
    {
        if (_isDiveRoll == true)
            return;

        if (co_HitDown.IsNull() == false) StopCoroutine(co_HitDown);
        co_HitDown = StartCoroutine(HitDown(attacker, addDamge));
    }

    // 레벨업 시 이펙트 발동
    private Coroutine co_LevelUp;
    public void LevelUpEffect()
    {
        if (co_LevelUp.IsNull() == false) StopCoroutine(co_LevelUp);
        co_LevelUp = StartCoroutine(LevelUpCoroutine());
    }

#region State 패턴

    protected override void UpdateIdle()
    {
        // TODO : 가만히 있을때의 모션 있으면 사용
        if (_stopAttack == false)
            StopAttack();
    }

    private float _scanTargetRange = 1.5f;
    protected override void UpdateMoving()
    {
        // 이동한 곳에 타겟이 있으면 멈추기
        if (_lockTarget.IsNull() == false)
        {
            float distance = (_lockTarget.transform.position - transform.position).magnitude;
            if (distance <= _scanTargetRange)
            {
                State = Define.State.Idle;
                
                // 타겟이 NPC라면 상호작용
                if (_lockTarget.GetComponent<NpcController>().IsNull() == false)
                    _lockTarget.GetComponent<NpcController>().GetInteract();

                return;
            }
        }

        // 타겟 대상을 클릭했을 때 콜라이더 위쪽을 클릭하게 된다면 그쪽을 바라보고 달리기 때문에 y값을 0으로 준다.
        _destPos.y = 0; 

        // 타겟과의 거리
        dir = _destPos - transform.position;

        // 0.1만큼 가깝다면 멈추기
        if (dir.magnitude < 0.1f)
            State = Define.State.Idle;
        else
        {
            // 가는 중에 벽이 있으면 멈추기
            if (BlockCheck() == true)
            {
                if (Input.GetMouseButton(0) == false)
                    State = Define.State.Idle;
                return;
            }

            // 회전
            float moveDist = Mathf.Clamp(Managers.Game.MoveSpeed * Time.deltaTime, 0, dir.magnitude);
            
            transform.position += dir.normalized * moveDist;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 20f * Time.deltaTime);
        }
    }

    private float diveTime = 0.8f;
    protected override void UpdateDiveRoll()
    {
        // 구르기 타이머
        currentDiveTime += Time.deltaTime;
        if (currentDiveTime >= diveTime)
        {
            ClearDiveRoll();
            return;
        }

        // 공격 중지
        StopAttack();

        // 도착 위치 받기
        _destPos = GetMousePoint();
        float moveDist = Mathf.Clamp(Managers.Game.MoveSpeed * Time.deltaTime, 0, dir.magnitude);

        // 이동
        transform.position += dir.normalized * moveDist;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 20f * Time.deltaTime);

        // 벽 확인
        if (BlockCheck() == true)
        {
            _isDiveRoll = false;
            Managers.Game.MoveSpeed = 5;
            return;
        }
    }

    protected override void UpdateAttack()
    {
        _attackCloseTime += Time.deltaTime;

        // 공격이 시간이 끝나면 종료 || 공격 했는데 가만히 있다면
        if ((anim.GetCurrentAnimatorStateInfo(0).IsTag("Attack") == true &&
            _attackCloseTime > 0.94f && _onComboAttack == false) || 
            (anim.GetCurrentAnimatorStateInfo(0).IsName("WAIT") == true &&
             anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.2f))
        {
            StopAttack();
            State = Define.State.Idle;
            return;
        }
    }

#endregion

#region 마우스 입력

    // 마우스 클릭
    private void OnMouseEvent(Define.MouseEvent evt)
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

    private float minDistance = 0.3f;
    private void GetMouseEvent(Define.MouseEvent evt)
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
                        
                        // 클릭 장소에 파란 원 활성화시키기
                        clickMoveEffect.SetActive(false);
                        clickMoveEffect.SetActive(true);
                        clickMoveEffect.transform.position = _destPos;

                        // 클릭 위치에 타겟이 있다면 저장
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
                    // 공격 상태가 아니라면
                    if (_stopAttack == true)
                    {
                        // 멈추고 있으면 움직이기
                        if (State == Define.State.Idle)
                            State = Define.State.Moving;

                        if (_lockTarget.IsNull() == false)
                            _destPos = _lockTarget.transform.position;
                        else if (raycastHit)
                            _destPos = hit.point;
                    }
                }
                break;
            // 왼쪽 클릭 시 공격
            case Define.MouseEvent.LeftDown:
                {
                    // 무기가 있다면 공격 가능
                    if (Managers.Game.CurrentWeapon.IsNull() == false)
                    {
                        _stopAttack = false;
                        _destPos = hit.point;
                        _destPos.y = 0;
                        OnAttack();
                    }
                }
                break;
            // 왼쪽 누르는 중이면 다음 공격 진행
            case Define.MouseEvent.LeftPress:
                {
                    if (_stopAttack == false)
                    {
                        _destPos = hit.point;
                        _destPos.y = 0;
                        OnAttack();
                    }
                }
                break;
        }
    }

    private bool _onAttack = false;         // 공격 여부 체크
    private bool _onComboAttack = false;    // 콤보 공격 여부 체크
    private void OnAttack()
    {
        // 콤보 체크 (공격 중에 다음 공격을 할 것인지?)
        if (_onAttack &&
            anim.GetCurrentAnimatorStateInfo(0).IsTag("Attack") &&
            anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.3f)
        {
            _onComboAttack = true;
        }

        // 공격!
        if (_onAttack == false)
        {
            State = Define.State.Attack;
            _onAttack = true;

            dir = _destPos - transform.position;
            transform.rotation = Quaternion.LookRotation(GetMousePoint() - transform.position);
        }
    }

#endregion

#region 키입력

    // 키보드 클릭
    private void OnKeyEvent()
    {
        if (State == Define.State.Die)
            return;

        // 구르지 않을 때 가능
        if (_isDiveRoll == false)
        {
            GetDiveRoll();  // 구르기
            GetSkill();     // 스킬
        }

        GetUseItem();       // 아이템 사용
        GetPickUp();        // 아이템 줍기
    }

    // F Key로 아이템 줍기
    [SerializeField]
    private float itemMaxRadius = 5f;
    private void GetPickUp()
    {
        // 주변 아이템 탐색
        Collider[] colliders = Physics.OverlapSphere(transform.position, itemMaxRadius, 1 << 12); // 12 : Item
        
        // F 키를 누르면 줍기
        if (Input.GetKeyDown(KeyCode.F))
        {
            for(int i=0; i<colliders.Length; i++)
            {
                ItemPickUp _item = colliders[i].GetComponent<ItemPickUp>();
                if (_item.IsNull() == false)
                {
                    // 인벤에 넣기
                    if (Managers.Game._playScene._inventory.AcquireItem(_item.item, _item.itemCount) == true)
                        Destroy(colliders[i].gameObject);

                    return;
                }
            }
        }
    }

    // Space Bar로 구르기
    private void GetDiveRoll()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (BlockCheck() == true)
                return;
            
            // 마나 체크
            if (Managers.Game.Mp < 10)
            {
                Managers.UI.MakeSubItem<UI_Guide>().SetInfo("마나가 부족합니다.", Color.blue);
                return;
            }
                
            _isDown = false;
            _isDiveRoll = true;

            // 넘어진 상태라면 취소시키기
            StopCoroutine(HitDown(null));

            State = Define.State.DiveRoll;

            Managers.Game.Mp -= 10;

            _destPos = GetMousePoint();
            dir = _destPos - transform.position;
            
            Managers.Game.MoveSpeed = 8;

            EffectClose();
        }
    }

    // 번호키를 눌러 소비 아이템 사용
    private void GetUseItem()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            Managers.Game._playScene.UsingItem(1);
        else if (Input.GetKeyDown(KeyCode.Alpha2))
            Managers.Game._playScene.UsingItem(2);
    }

    // 스킬 사용 (Q, W, E, A, S, D, R)
    private void GetSkill()
    {
        // 스킬 사용 중이면 x
        if (State == Define.State.Skill || _isDown == true)
            return;

        // 무기가 없으면 스킬 사용 불가
        if (Managers.Game.CurrentWeapon.IsNull() == true)
            return;

        // 스킬 진행
        if (Input.GetKeyDown(KeyCode.Q))        OnSkill(GetSkill(Define.KeySkill.Q));
        else if (Input.GetKeyDown(KeyCode.W))   OnSkill(GetSkill(Define.KeySkill.W));
        else if (Input.GetKeyDown(KeyCode.E))   OnSkill(GetSkill(Define.KeySkill.E));
        else if (Input.GetKeyDown(KeyCode.A))   OnSkill(GetSkill(Define.KeySkill.A));
        else if (Input.GetKeyDown(KeyCode.S))   OnSkill(GetSkill(Define.KeySkill.S));
        else if (Input.GetKeyDown(KeyCode.D))   OnSkill(GetSkill(Define.KeySkill.D));
        else if (Input.GetKeyDown(KeyCode.R))   OnSkill(GetSkill(Define.KeySkill.R));
    }

    // 스킬 진행
    private void OnSkill(SkillData skill)
    {
        if (skill.IsNull() == true)
        {
            Debug.Log("등록된 스킬이 없습니다!");
            return;
        }

        if (skill.isCoolDown == true)
        {
            Debug.Log("쿨타임 중입니다.");
            return;
        }

        // 마나 체크
        if (skill.skillConsumMp > Managers.Game.Mp)
        {
            Managers.UI.MakeSubItem<UI_Guide>().SetInfo("마나가 부족합니다.", Color.blue);
            return;
        }

        // 일반 공격 중지
        StopAttack();

        // 마우스 방향으로 회전
        _destPos = GetMousePoint();
        dir = _destPos - transform.position;
        transform.rotation = Quaternion.LookRotation(dir);

        currentSkill = skill;
        
        // 스킬 이펙트 찾기
        foreach(EffectData effect in effects)
        {
            if (currentSkill.skillId == effect.id)
            {
                currentEffect = effect.gameObject;
                break;
            }
        }

        // 스킬 진행
        State = Define.State.Skill;
        anim.CrossFade("SKILL"+currentSkill.skillId, 0.1f, -1, 0);

        currentSkill.isCoolDown = true;
        Managers.Game.Mp -= currentSkill.skillConsumMp;

        // 스킬 이펙트 활성화
        currentEffect.SetActive(true);
    }

#endregion

    // 플레이어 넘어지기
    private IEnumerator HitDown(MonsterStat attacker, int addDamge = 0)
    {   
        Managers.Game.OnAttacked(attacker, addDamge);

        if (State == Define.State.Die)
            yield break;

        State = Define.State.Down;
        _isDown = true;

        // 공격자 바라보기
        Vector3 dir = attacker.transform.position - transform.position;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 1);

        yield return new WaitForSeconds(2f);

        if (_isDiveRoll == false)
            State = Define.State.Idle;
            
        _isDown = false;
    }

    // 레벨업 이펙트 효과
    private IEnumerator LevelUpCoroutine()
    {
        GameObject effect = Managers.Resource.Instantiate("Effect/LevelUpEffect", this.transform);
        effect.transform.localPosition = Vector3.zero;

        yield return new WaitForSeconds(4f);

        Managers.Resource.Destroy(effect);
    }

    // 마우스 Ray 위치 반환
    private Vector3 GetMousePoint()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(ray, out hit, 150f, _mask);
        
        Vector3 hitPoint = hit.point;
        hitPoint.y = 0;
        return hitPoint;
    }

    // 해당 키 스킬 반환 
    private SkillData GetSkill(Define.KeySkill keySkill)
    {
        SkillData skill;
        if (Managers.Game.SkillBarList.TryGetValue(keySkill, out skill) == false)
            return null;

        return skill;
    }

    // 전방 Block 체크하여 멈추기 (1.0f 거리에서 멈추기)
    private bool BlockCheck()
    {
        if (Physics.Raycast(transform.position + (Vector3.up * 0.5f), dir, 1.0f, 1 << 10)) // 10 : Block
            return true;

        return false;
    }

    // [ Anim Event ]
    // 공격이 끝나면 발동.
    private void ExitAttack()
    {
        if (_onComboAttack == true)
        {
            State = Define.State.Attack;
            _onComboAttack = false;

            dir = _destPos - transform.position;
            transform.rotation = Quaternion.LookRotation(GetMousePoint() - transform.position);
        }
    }

    // [ Anim Event ]
    // 구르기가 끝나면 발동.
    private void EventDiveRoll()
    {
        ClearDiveRoll();
    }

    // [ Anim Event ]
    // 스킬 끝날 때 발동
    private void EventEndSkill()
    {
        EffectClose();
        ClearDiveRoll();
        State = Define.State.Idle;
    }

    // 공격 중지
    private void StopAttack()
    {
        _onAttack = false;
        _onComboAttack = false;
        _stopAttack = true;
        _attackCloseTime = 0;
        attackNumber = 1;
    }

    // 구르기 초기화
    private void ClearDiveRoll()
    {
        _isDiveRoll = false;
        Managers.Game.MoveSpeed = 5;
        currentDiveTime = 0f;
        State = Define.State.Idle;
    }

    // 스킬 이펙트 비활성화
    private void EffectClose()
    {
        if (currentEffect.IsFakeNull() == true)
            return;
        
        currentEffect.GetComponent<EffectData>().EffectDisableDelay();
    }

    private void SetPart()
    {
        // 캐릭터 파츠 가져오기
        GameObject goChild = Util.FindChild(gameObject, "Modular_Characters");
        foreach(Transform child in goChild.GetComponentsInChildren<Transform>())
        {
            // 캐릭터의 커스텀 파츠 저장
            if (child.CompareTag("Custom"))
            {
                string result = Regex.Replace(child.name, "Base", "");
                Define.DefaultPart partType = (Define.DefaultPart)System.Enum.Parse(typeof(Define.DefaultPart), result);

                SetSkinned(partType, child);
                continue;
            }

            // 장비 파츠 가져오기
            if (child.CompareTag("Equipment"))
            {
                // 기본 옷이라면 커스텀했던 옷 입혀주기
                if (child.name.Contains("Defualt") == true)
                {
                    string defualtResult = Regex.Replace(child.name, "Defualt", "");
                    defualtResult = Regex.Replace(defualtResult, @"\d", "");
                    Define.DefaultPart partType = (Define.DefaultPart)System.Enum.Parse(typeof(Define.DefaultPart), defualtResult);
                    
                    SetSkinned(partType, child);
                }

                string result = Regex.Replace(child.name, @"\D", "");
                int id = int.Parse(result);

                // 아이템 안에 장비 파츠 저장
                ArmorItemData armor = Managers.Data.Item[id] as ArmorItemData;
                if (armor.charEquipment.IsNull() == true)
                    armor.charEquipment = new List<GameObject>();

                armor.charEquipment.Add(child.gameObject);

                // 플레이어 안에서 장비 파츠 저장
                List<GameObject> equipList;
                if (charEquipment.TryGetValue(id, out equipList) == false)
                {
                    equipList = new List<GameObject>();
                    charEquipment.Add(id, equipList);
                }

                equipList.Add(child.gameObject);

                child.gameObject.SetActive(false);
            }
        }
        
        // 장착할 무기 객체 아이템 안에 저장
        foreach(Transform child in waeponObjList.transform)
        {
            string result = Regex.Replace(child.name, @"\D", "");
            int id = int.Parse(result);

            WeaponItemData weapon = Managers.Data.Item[id] as WeaponItemData;
            weapon.charEquipment = child.gameObject;

            child.gameObject.SetActive(false);
        }
    }

    // SkinnedMeshReaderer 변경
    private void SetSkinned(Define.DefaultPart partType, Transform go)
    {
        SkinnedMeshRenderer objSkinned = go.GetComponent<SkinnedMeshRenderer>();

        SkinnedData skinnedInfo = Managers.Game.DefaultPart[partType];

        // 파츠를 가지고 있는 Model FBX를 찾아 파츠 이름 검색하여 Mesh 받기
        GameObject meshObj = Managers.Resource.Load<GameObject>("Art/PolygonFantasyHeroCharacters/Models/ModularCharacters");

        objSkinned.sharedMesh = Util.FindChild<SkinnedMeshRenderer>(meshObj, skinnedInfo.sharedMeshName, true).sharedMesh;
        objSkinned.localBounds = skinnedInfo.bounds;
        objSkinned.rootBone = Util.FindChild<Transform>(rootBone, skinnedInfo.rootBoneName, true);

        Transform[] newBones = new Transform[skinnedInfo.bones.Count];
        for(int i=0; i<skinnedInfo.bones.Count; i++)
        {
            newBones[i] = Util.FindChild<Transform>(rootBone, skinnedInfo.bones[i], true);
        }
        
        objSkinned.bones = newBones;
    }
}