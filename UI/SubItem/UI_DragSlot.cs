using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
[ 이동 Slot 스크립트 ]
1. Slot을 마우스로 잡고 드래그할 때 이동을 표현한다.
*/

public class UI_DragSlot : MonoBehaviour
{
    public static UI_DragSlot instance;

    public UI_Slot dragSlotItem;

    // 아이템 이미지
    public Image icon;

    void Start()
    {
        instance = this;
    }

    // 드래그 할 경우 활성화
    public void DragSetImage(Image _icon)
    {
        Managers.UI.SetOrder(GetComponent<Canvas>());
        icon.sprite = _icon.sprite;
        SetColor(1);
    }

    public void SetColor(float _alpha)
    {
        Color color = icon.color;
        color.a = _alpha;
        icon.color = color;
    }
}
