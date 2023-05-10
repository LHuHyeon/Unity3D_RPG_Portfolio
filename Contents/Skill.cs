using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill
{
    public int skillId;
    public string skillName;
    public int Power;           // 공격 파워
    public int Range;           // 거리
    public int CollTile;        // 쿨타임
    public int ConsumMp;        // 사용 Mp
    public string discription;  // 설명
    
    public Sprite skillIcon;
}