using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
[ 아이템 줍기 스크립트 ]
1. 땅에 떨어져 있는 아이템에 들어있는 스크립트.
2. 플레이어가 다가오면 이름을 표시한다.
3. PlayerController에서 F를 눌렀을 때 ItemPickUp Class를 확인하여 아이템을 줍는다.
*/

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
