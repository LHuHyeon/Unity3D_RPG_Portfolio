using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterStat : MonoBehaviour
{
    [SerializeField] protected int _id;
    [SerializeField] protected string _name = "Monster";
    [SerializeField] protected int _hp;
    [SerializeField] protected int _maxHp;
    [SerializeField] protected int _attack;
    [SerializeField] protected int _dropExp;
    [SerializeField] protected int _dropGold;
    [SerializeField] protected int _dropItemId = 0;
    [SerializeField] protected float _movespeed;

    public int Id { get { return _id; } set { _id = value; } }
    public string Name { get { return _name; } set { _name = value; } }
    public int Hp { get { return _hp; } set { _hp = Mathf.Clamp(value, 0, MaxHp); } }
    public int MaxHp { get { return _maxHp; } set { _maxHp = value; Hp = MaxHp; } }
    public int Attack { get { return _attack; } set { _attack = value; } }
    public int DropExp { get { return _dropExp; } set { _dropExp = value; } }
    public int DropGold { get { return _dropGold; } set { _dropGold = value; } }
    public int DropItemId { get { return _dropItemId; } set { _dropItemId = value; } }
    public float MoveSpeed { get { return _movespeed; } set { _movespeed = value; } }
    
    void Start()
    {
        _monster = GetComponent<MonsterController>();
    }

    // 공격을 받았을 때
    MonsterController _monster;
    public virtual void OnAttacked(int skillAttack=0)
    {
        _monster.State = Define.State.Hit;

        int damage;
        if (skillAttack != 0)
            damage = Mathf.Max(0, skillAttack);
        else 
            damage = Mathf.Max(0, Managers.Game.Attack);
            
        Hp -= damage;
        Debug.Log("Hit Damage : " + damage + "\nSTR : " + Managers.Game.STR);

        HitEffect(damage);

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
        Managers.Game.Exp += _dropExp;

        OnDropItem();
        _monster.BattleClose();
    }

    // 드랍 아이템
    void OnDropItem()
    {
        List<int> idList = Managers.Data.DropItem[_dropItemId];

        // 아이탬 개수 0~2 + Luk (최대 5개까지)
        int maxCount = Mathf.Clamp(2 + Managers.Game.LUK, 0, 5);
        for(int i=0; i<Random.Range(0, maxCount); i++)
        {
            int randomId = Random.Range(0, idList.Count-1);

            GameObject go = Managers.Resource.Instantiate(Managers.Data.Item[idList[randomId]].itemObject);
            ItemPickUp goData = go.GetOrAddComponent<ItemPickUp>();

            goData.id = idList[randomId];

            float ranPos = Random.Range(-0.5f, 0.5f);
            go.transform.position = transform.position * ranPos;
        }
    }

    // 피격 데미지 출력
    void HitEffect(int damage)
    {
        UI_HitEffect hitObject = Managers.UI.MakeWorldSpaceUI<UI_HitEffect>(gameObject.transform);
        hitObject.hitText.text = damage.ToString();

        float randomX = Random.Range(-0.5f, 0.5f);
        float valueY = GetComponent<Collider>().bounds.size.y * 0.8f;
        hitObject.transform.position = transform.position + new Vector3(randomX, valueY, 0);
    }
}
