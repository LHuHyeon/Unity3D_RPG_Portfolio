using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Data
{
    #region Stat

    [Serializable]
    public class Stat
    {
        public int level;
        public int maxHp;
        public int attack;
        public int totalExp;
    }

    // TODO : 레벨업 관련으로 수정
    [Serializable]
    public class StatData : ILoader<int, Stat>
    {
        public List<Stat> stats = new List<Stat>();

        public Dictionary<int, Stat> MakeDic()
        {
            Dictionary<int, Stat> dict = new Dictionary<int, Stat>();

            // List를 Dictionary로 변환
            foreach(Stat stat in stats){
                dict.Add(stat.level, stat);
            }

            return dict;
        }

        public bool Validate()
        {
            throw new NotImplementedException();
        }
    }

    #endregion
}
