using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmorItemData : EquipmentData
{
    public Define.ArmorType armorType = Define.ArmorType.Unknown;
    public int defnece=0;
    public int hp=0;
    public int mp=0;
    public int moveSpeed=0;

    public int addDefnece=0;
    public int addHp=0;
    public int addMp=0;
    public int addMoveSpeed=0;

    public List<GameObject> charEquipment;
}
