using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectData : MonoBehaviour
{
    public int id=0;
    public float disableDelayTime=0;    // effect 전용 비활성화 딜레이

    UI_NameBar nameBarUI = null;

    float scanRange = 5f;

    void Start()
    {
        if (gameObject.CompareTag("Item"))
        {
            nameBarUI = Managers.UI.MakeWorldSpaceUI<UI_NameBar>(transform);
            nameBarUI.nameText = Managers.Data.Item[id].itemName;
        }
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
