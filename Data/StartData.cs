using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

public class StartData
{
    [XmlAttribute]
    public int Id;
    [XmlAttribute]
    public int exp;
    [XmlAttribute]
    public int level;
    [XmlAttribute]
    public int maxHp;
    [XmlAttribute]
    public int maxMp;
    [XmlAttribute]
    public int STR;
    [XmlAttribute]
    public int Speed;
    [XmlAttribute]
    public int LUK;
    [XmlAttribute]
    public int gold;
}
