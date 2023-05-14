using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill
{
    public int skillId;
    public string skillName;
    public int minLevel;        // 최소 레벨
    public List<int> powerList; // 공격력 리스트
    public int CollTime;        // 쿨타임
    public int ConsumMp;        // 사용 Mp
    public string discription;  // 설명
    
    public Sprite skillIcon;
}