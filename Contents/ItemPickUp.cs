using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickUp : MonoBehaviour
{
    public ItemData item;
    public int itemCount = 1;   // 아이템 전용 개수

    UI_NameBar nameBarUI = null;

    float scanRange = 5f;

    void Start()
    {
        nameBarUI = Managers.UI.MakeWorldSpaceUI<UI_NameBar>(transform);
        if (itemCount > 1)
            nameBarUI.nameText = item.itemName + $" ({itemCount})";
        else
            nameBarUI.nameText = item.itemName;

        nameBarUI.objectType = Define.WorldObject.Item;
    }

    void FixedUpdate()
    {
        if (nameBarUI != null)
        {
            float distance = (Managers.Game.GetPlayer().transform.position - transform.position).magnitude;
            if (distance <= scanRange)
                nameBarUI.gameObject.SetActive(true);
            else
                nameBarUI.gameObject.SetActive(false);
        }
    }
}
