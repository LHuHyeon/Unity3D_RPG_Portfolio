using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * File :   ItemPickUp.cs
 * Desc :   땅에 떨어진 아이템의 정보를 가지고 있으며 플레이어와 가까우면 이름 생성
 *
 & Functions
 &  : Start()       - 이름바 생성
 &  : FixedUpdate() - 플레이어 근접 시 이름바 활성화
 *
 */

public class ItemPickUp : MonoBehaviour
{
    public  ItemData    item;
    public  int         itemCount = 1;      // 아이템 전용 개수

    private float       scanRange = 5f;     // 플레이어 스캔 거리

    private UI_NameBar  nameBarUI = null;

    void Start()
    {
        // 이름바 생성 및 자식으로 배치
        nameBarUI = Managers.UI.MakeWorldSpaceUI<UI_NameBar>(transform);
        if (itemCount > 1)
            nameBarUI.nameText = item.itemName + $" ({itemCount})";
        else
            nameBarUI.nameText = item.itemName;

        nameBarUI.nameText += " [F]";
    }

    void FixedUpdate()
    {
        // 이름바 Null Check
        if (nameBarUI.IsNull() == false)
        {
            // 플레이어 Null Check
            if (Managers.Game.GetPlayer().IsNull() == true)
                return;
                
            // 플레이어와 거리 체크
            float distance = (Managers.Game.GetPlayer().transform.position - transform.position).magnitude;

            // scanRange만큼 가까우면 활성화
            if (distance <= scanRange)
                nameBarUI.gameObject.SetActive(true);
            else
                nameBarUI.gameObject.SetActive(false);
        }
    }
}
