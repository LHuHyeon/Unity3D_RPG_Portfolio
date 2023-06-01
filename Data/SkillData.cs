using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillData
{
    public int skillId;
    public string skillName;
    public int minLevel;
    public int skillCoolDown;
    public int skillConsumMp;
    public bool isCoolDown = false;
    public string discription;
    public Sprite skillSprite;
    public List<int> powerList;
}