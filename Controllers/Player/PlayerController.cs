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
    bool _stopAttack = true;    // 공격 가능 여부
    
    bool _isDiveRoll = false;   // 구르기 여부
    bool _isDown = false;       // 넘어진 상태 여부

    // Click LayerMask
    int _mask = (1 << (int)Define.Layer.Ground) | (1 << (int)Define.Layer.Monster) | (1 << (int)Define.Layer.Npc);

    [SerializeField]
    GameObject rootBone;                // SkinnedMeshRenderer 대표 뼈대

    public GameObject clickMoveEffect;  // 이동 클릭 활성화 이펙트

    // 이펙트 관리 변수
    [SerializeField]
    private List<EffectData> effects = new List<EffectData>();
    public GameObject currentEffect;

    // 현재 스킬
    public SkillData currentSkill;

    // 장비 오브젝트 저장 (입는 장비)
    public Dictionary<int, List<GameObject>> charEquipment;

    // 무기 오브젝트 저장 객체
    public GameObject waeponObjList;

    public override void Init()
    {
        WorldObjectType = Define.WorldObject.Player;
        
        charEquipment = new Dictionary<int, List<GameObject>>();

        anim = GetComponent<Animator>();
        currentEffect = null;

        Managers.Input.KeyAction -= OnKeyEvent;
        Managers.Input.KeyAction += OnKeyEvent;
        Managers.Input.MouseAction -= OnMouseEvent;
        Managers.Input.MouseAction += OnMouseEvent;

        SetInfo();
    }

    void SetInfo()
    {
        // 캐릭터 파츠 가져오기
        GameObject goChild = Util.FindChild(gameObject, "Modular_Characters");
        foreach(Transform child in goChild.GetComponentsInChildren<Transform>())
        {
            // 캐릭터의 기본 부위 저장 (+얼굴 커스텀)
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
    void SetSkinned(Define.DefaultPart partType, Transform go)
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

#region State 패턴

    // 멈춤 상태
    protected override void UpdateIdle()
    {
        // TODO : 가만히 있을때의 모션 있으면 사용
        if (_stopAttack == false)
            StopAttack();
    }

    Vector3 dir;
    float _scanRange = 1.5f;
    // 이동 상태
    protected override void UpdateMoving()
    {
        // 이동한 곳에 타겟이 있으면 멈추기
        if (_lockTarget.IsNull() == false)
        {
            float distance = (_lockTarget.transform.position - transform.position).magnitude;
            if (distance <= _scanRange)
            {
                State = Define.State.Idle;
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
            if (BlockCheck() == true){
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

    // 구르기 상태
    protected override void UpdateDiveRoll()
    {
        StopAttack();

        _destPos = GetMousePoint();

        float moveDist = Mathf.Clamp(Managers.Game.MoveSpeed * Time.deltaTime, 0, dir.magnitude);

        transform.position += dir.normalized * moveDist;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 20f * Time.deltaTime);

        if (BlockCheck() == true)
        {
            _isDiveRoll = false;
            Managers.Game.MoveSpeed = 5;
            return;
        }
    }

    float _attackCloseTime = 0;
    // 공격 상태
    protected override void UpdateAttack()
    {
        _attackCloseTime += Time.deltaTime;

        // 공격이 시간이 끝나면 종료 || 공격이 끝났는데 다른 애니메이션에 있다면 종료
        if ((anim.GetCurrentAnimatorStateInfo(0).IsTag("Attack") == true &&
            _attackCloseTime > 0.94f && _onComboAttack == false) || 
            (anim.GetCurrentAnimatorStateInfo(0).IsTag("Attack") == false &&
            _attackCloseTime > 0.99f && _onComboAttack == false))
        {
            StopAttack();
            State = Define.State.Idle;
            return;
        }

        dir = _destPos - transform.position;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 20f * Time.deltaTime);
    }

    // 피격 받기 (넘어지는 공격에 호출됨)
    public void OnHitDown(MonsterStat attacker, int addDamge = 0)
    {
        if (_isDiveRoll == true)
            return;

        StopCoroutine(HitDown(null));
        StartCoroutine(HitDown(attacker, addDamge));
    }

    // 플레이어 피격 넘어지기
    public IEnumerator HitDown(MonsterStat attacker, int addDamge = 0)
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

#endregion

#region 마우스 입력

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

#endregion

#region 연속 공격 스크립트

    bool _onAttack = false;         // 공격 여부 체크
    bool _onComboAttack = false;    // 콤보 공격 여부 체크
    int attackClipNumber = 0;       // 애니메이션 number
    string[] _attackClipList = new string[] {"ATTACK1", "ATTACK2"};
    void OnAttack()
    {
        // 콤보 체크 (공격 중에 다음 공격을 할 것인지?)
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
    // 공격이 끝나면 발동.
    public void ExitAttack()
    {
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

    // 공격 중지
    void StopAttack()
    {
        _onAttack = false;
        _onComboAttack = false;
        _stopAttack = true;
        _attackCloseTime = 0;
        attackClipNumber = 0;
    }

#endregion

#region 키입력

    // 키보드 클릭
    void OnKeyEvent()
    {
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
    float itemMaxRadius = 5f;
    void GetPickUp()
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
                    Managers.Game._playScene._inventory.AcquireItem(_item.item, _item.itemCount);
                    Destroy(colliders[i].gameObject);

                    return;
                }
            }
        }
    }

    // [ Anim Event ]
    // 구르기가 끝나면 발동.
    public void EventDiveRoll()
    {
        _isDiveRoll = false;
        Managers.Game.MoveSpeed = 5;
        State = Define.State.Idle;
    }

    // Space Bar로 구르기
    void GetDiveRoll()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (BlockCheck() == true)
                return;
                
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
    void GetUseItem()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            Managers.Game._playScene.UsingItem(1);
        else if (Input.GetKeyDown(KeyCode.Alpha2))
            Managers.Game._playScene.UsingItem(2);
    }

    // 스킬 사용 (Q, W, E, A, S, D, R)
    void GetSkill()
    {
        // 스킬 사용 중이면 x
        if (State == Define.State.Skill || _isDown == true)
            return;

        // 무기가 없으면 스킬 사용 불가
        if (Managers.Game.CurrentWeapon.IsNull() == true)
            return;

        // 스킬 진행
        if (Input.GetKeyDown(KeyCode.Q))
            OnSkill(Managers.Game.GetSkill(Define.KeySkill.Q));
        else if (Input.GetKeyDown(KeyCode.W))
            OnSkill(Managers.Game.GetSkill(Define.KeySkill.W));
        else if (Input.GetKeyDown(KeyCode.E))
            OnSkill(Managers.Game.GetSkill(Define.KeySkill.E));
        else if (Input.GetKeyDown(KeyCode.A))
            OnSkill(Managers.Game.GetSkill(Define.KeySkill.A));
        else if (Input.GetKeyDown(KeyCode.S))
            OnSkill(Managers.Game.GetSkill(Define.KeySkill.S));
        else if (Input.GetKeyDown(KeyCode.D))
            OnSkill(Managers.Game.GetSkill(Define.KeySkill.D));
        else if (Input.GetKeyDown(KeyCode.R))
            OnSkill(Managers.Game.GetSkill(Define.KeySkill.R));
    }

    // 스킬 진행
    void OnSkill(SkillData skill)
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
            return;

        // 일반 공격 중지
        StopAttack();

        // 마우스 방향으로 회전
        _destPos = GetMousePoint();
        dir = _destPos - transform.position;
        transform.rotation = Quaternion.LookRotation(dir);

        currentSkill = skill;
        
        // 스킬 이펙트 찾기
        foreach(EffectData value in effects)
        {
            if (currentSkill.skillId == value.id)
            {
                currentEffect = value.gameObject;
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

    // [ Anim Event ]
    // 스킬 끝날 때 발동
    public void EventEndSkill()
    {
        EffectClose();
        State = Define.State.Idle;
    }

#endregion

    // 마우스 Ray 위치 반환
    public Vector3 GetMousePoint()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(ray, out hit, 150f, _mask);
        
        Vector3 hitPoint = hit.point;
        hitPoint.y = 0;
        return hitPoint;
    }

    // 스킬 이펙트 비활성화
    public void EffectClose()
    {
        if (currentEffect.IsNull() == true)
            return;
        
        if (currentEffect.GetComponent<EffectData>().disableDelayTime == 0)
            currentEffect.SetActive(false);
        else
            StartCoroutine(EffectDisableDelayTime());
    }

    // 플레이어가 움직이더라도 스킬 이펙트가 활성화되야 한다면 사용
    IEnumerator EffectDisableDelayTime()
    {
        Transform effectParent = currentEffect.transform.parent;   // 이펙트 부모
        Vector3 effectPos = currentEffect.transform.localPosition; // 이펙트 위치

        // 부모 빠져나오기
        currentEffect.transform.SetParent(null);
    
        // 이펙트 비활성화 기다리기
        yield return new WaitForSeconds(currentEffect.GetComponent<EffectData>().disableDelayTime);

        // 원위치 이동 후 비활성화
        currentEffect.transform.SetParent(effectParent);
        currentEffect.transform.localPosition = effectPos;
        currentEffect.transform.localRotation = Quaternion.identity;

        currentEffect.SetActive(false);
    }

    // 전방 Block 체크하여 멈추기 (1.0f 거리에서 멈추기)
    bool BlockCheck()
    {
        if (Physics.Raycast(transform.position + (Vector3.up * 0.5f), dir, 1.0f, 1 << 10)) // 10 : Block
            return true;

        return false;
    }

    Coroutine co;
    // 레벨업 시 이펙트 발동
    public void LevelUpEffect()
    {
        if (co.IsNull() == false) StopCoroutine(co);
        co = StartCoroutine(LevelUpCoroutine());
    }

    IEnumerator LevelUpCoroutine()
    {
        GameObject effect = Managers.Resource.Instantiate("Effect/LevelUpEffect", this.transform);
        effect.transform.localPosition = Vector3.zero;

        yield return new WaitForSeconds(4f);

        Managers.Resource.Destroy(effect);
    }
}