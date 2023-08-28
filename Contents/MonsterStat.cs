using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * File :   MonsterStat.cs
 * Desc :   몬스터 기본 정보, 피격, 드랍 아이템을 관리
 *
 & Functions
 &  [Public]
 &  : OnAttacked()  - 공격 받을 때 호출
 &
 &  [Protected]
 &  : OnDead()      - 사망 시 호출
 &
 &  [Private]
 &  : OnDropItem()  - 드랍 아이템 관리
 &  : HitEffect()   - 피격 이펙트 생성
 *
 */

public class MonsterStat : MonoBehaviour
{
    [SerializeField] protected int      _id;
    [SerializeField] protected string   _name = "Monster";
    [SerializeField] protected int      _hp;
    [SerializeField] protected int      _maxHp;
    [SerializeField] protected int      _attack;
    [SerializeField] protected int      _dropExp;
    [SerializeField] protected int      _dropGold;
    [SerializeField] protected int      _dropItemId = 0;
    [SerializeField] protected float    _movespeed;

    public int      Id          { get { return _id; } set { _id = value; } }
    public string   Name        { get { return _name; } set { _name = value; } }
    public int      Hp          { get { return _hp; } set { _hp = Mathf.Clamp(value, 0, MaxHp); } }
    public int      MaxHp       { get { return _maxHp; } set { _maxHp = value; Hp = MaxHp; } }
    public int      Attack      { get { return _attack; } set { _attack = value; } }
    public int      DropExp     { get { return _dropExp; } set { _dropExp = value; } }
    public int      DropGold    { get { return _dropGold; } set { _dropGold = value; } }
    public int      DropItemId  { get { return _dropItemId; } set { _dropItemId = value; } }
    public float    MoveSpeed   { get { return _movespeed; } set { _movespeed = value; } }
    
    void Start()
    {
        _monster = GetComponent<MonsterController>();

        GameObject go;
        if (Managers.Data.Monster.TryGetValue(Id, out go) == true)
        {
            MonsterStat stat = go.GetComponent<MonsterStat>();
            _name = stat.Name;
            _hp = stat.Hp;
            _maxHp = stat.MaxHp;
            _attack = stat.Attack;
            _dropExp = stat.DropExp;
            _dropGold = stat.DropGold;
            _dropItemId = stat.DropItemId;
            _movespeed = stat.MoveSpeed;
        }
    }

    // 공격을 받았을 때
    MonsterController _monster;
    public virtual void OnAttacked(int skillAttack=0)
    {
        // 일반 몬스터만 피격 상태 진행
        if (_monster.monsterType == Define.MonsterType.Normal)
            _monster.State = Define.State.Hit;

        // Scene UI에 몬스터 정보 활성화
        Managers.Game._playScene.OnMonsterBar(this);

        int damage;
        // 스킬 데미지 체크
        if (skillAttack != 0)
            damage = Mathf.Max(0, skillAttack);
        else 
            damage = Mathf.Max(0, Managers.Game.Attack);
            
        // 체력 차감
        Hp -= damage;

        // 피격 이펙트 생성
        HitEffect(damage);

        // 체력이 0보다 작으면 사망
        if (Hp <= 0)
        {
            Hp = 0;
            OnDead();
        }
    }

    // 죽었을 때
    protected virtual void OnDead()
    {
        _monster.State = Define.State.Die;

        // 보상 반영
        Managers.Game.Exp += _dropExp;
        Managers.Game.Gold += _dropGold;

        // 퀘스트 정보 반영
        Managers.Game._playScene._quest.QuestTargetCount(gameObject);

        // Scene UI 몬스터 정보 비활성화
        Managers.Game._playScene.CloseMonsterBar();

        // 아이템 드랍
        OnDropItem();

        // 전투 종료
        _monster.BattleClose();
    }

    // 드랍 아이템
    private void OnDropItem()
    {
        // DataManager에서 DropItem List가져오기
        List<int> itemList = Managers.Data.DropItem[_dropItemId];
        
        // 아이탬 개수 0~2 + Luk (최대 5개까지)
        int maxCount = Mathf.Clamp(2 + Managers.Game.LUK, 0, 5);

        for(int i=0; i<Random.Range(0, maxCount); i++)
        {
            // Random으로 아이템 id 뽑기
            int randomId = Random.Range(0, itemList.Count-1);

            // 아이템 소환
            ItemData item = Managers.Data.CallItem(itemList[randomId]);
            GameObject go = Managers.Resource.Instantiate(item.itemObject);

            // ItemPickUp 컴포넌트 붙이기
            ItemPickUp goData = go.GetOrAddComponent<ItemPickUp>();
            goData.item = item;

            // 드랍 위치 설정
            float ranPos = Random.Range(-0.5f, 0.5f);
            go.transform.position = new Vector3(transform.position.x + ranPos, 0f, transform.position.z + ranPos);
        }
    }

    // 피격 데미지 출력
    private void HitEffect(int damage)
    {
        // UI_HitEffect 생성 후 데미지 text 넣기
        UI_HitEffect hitObject = Managers.UI.MakeWorldSpaceUI<UI_HitEffect>(gameObject.transform);
        hitObject.hitText.text = damage.ToString();

        // 생성 위치 설정
        float randomX = Random.Range(-0.5f, 0.5f);
        float valueY = GetComponent<Collider>().bounds.size.y * 0.8f;
        hitObject.transform.position = transform.position + new Vector3(randomX, valueY, 0);
    }
}
