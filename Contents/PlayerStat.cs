using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStat : Stat
{
    [SerializeField] protected int _exp;
    [SerializeField] protected int _gold;
    [SerializeField] protected int _str;
    [SerializeField] protected int _luk;
    [SerializeField] protected int _statPoint;

    public int STR { get { return _str; } set { _str = value; } }
    public int LUK { get { return _luk; } set { _luk = value; } }
    public int StatPoint { get { return _statPoint; } set { _statPoint = value; } }

    public int Gold { get { return _gold; } set { _gold = value; } }
    public int Exp
    { 
        get { return _exp; }
        set
        { 
            _exp = value;

            int level = Level;

            while (true){
                LevelData stat;
                
                // 해당 Key에 Value가 존재 하는지 여부
                if (Managers.Data.Level.TryGetValue(level + 1, out stat) == false)
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
        if (Managers.Data.Start != null)
        {
            StartData startData = Managers.Data.Start;
            _level = startData.level;
            _exp = startData.exp;
            _str = startData.STR;
            _luk = startData.LUK;
            _movespeed = startData.MoveSpeed;
            _gold = startData.gold;
        }
        else
        {
            _level = 1;
            _exp = 0;
            _str = 3;
            _luk = 0;
            _movespeed = 5;
            _gold = 50;
        }

        // SetStat(_level);
    }

    public void SetStat(int level)
    {
        LevelData stat = Managers.Data.Level[level];

        _statPoint = stat.statPoint;
        _maxHp = stat.maxHp;
        _hp = _maxHp;
        _maxMp = stat.maxMp;
        _mp = _maxMp;
    }

    protected override void OnDead(Stat attacker)
    {
        Debug.Log("Player Dead");
    }
}
