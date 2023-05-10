using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStat : Stat
{
    [SerializeField] protected int _exp;
    [SerializeField] protected int _gold;

    public int Gold { get { return _gold; } set { _gold = value; } }
    public int Exp
    { 
        get { return _exp; }
        set
        { 
            _exp = value;

            int level = Level;

            while (true){
                Data.Stat stat;
                
                // 해당 Key에 Value가 존재 하는지 여부
                if (Managers.Data.StatDict.TryGetValue(level + 1, out stat) == false)
                    break;

                // 경험치가 다음 레벨 경험치보다 작은지 확인
                if (_exp < stat.totalExp)
                    break;
                
                level++;
            }

            if (level != Level){
                Level = level;
                SetStat(Level);
                Debug.Log("Level UP!!");
            }
        }
    }

    void Start()
    {
        _level = 1;

        _defense = 5;
        _movespeed = 5.0f;
        _gold = 0;
        _exp = 0;

        SetStat(_level);
    }

    public void SetStat(int level)
    {
        Dictionary<int, Data.Stat> dict = Managers.Data.StatDict;
        Data.Stat stat = dict[level];

        _hp = stat.maxHp;
        _maxHp = stat.maxHp;
        _attack = stat.attack;
    }

    protected override void OnDead(Stat attacker)
    {
        Debug.Log("Player Dead");
    }
}
