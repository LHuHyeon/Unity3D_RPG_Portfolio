using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * File :   SkillData.cs
 * Desc :   스킬 데이터
 */

[Serializable]
public class SkillData
{
    public int skillId;
    public string skillName;
    public int minLevel;
    public int skillCoolDown;
    public int skillConsumMp;
    public bool isCoolDown = false;
    public bool isLock = true;
    public string discription;
    public Sprite skillSprite;
    public List<int> powerList;
}