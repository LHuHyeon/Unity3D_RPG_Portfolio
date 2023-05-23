using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 아이템을 드래그하여 이동시킬 때 사용될 클래스
public class UI_DragSlot : MonoBehaviour
{
    public static UI_DragSlot instance;

    public UI_SlotItem dragSlotItem;

    // 아이템 이미지
    public Image itemImage;

    void Start()
    {
        instance = this;
    }

    // 드래그 할 경우 활성화
    public void DragSetImage(Image _itemImage)
    {
        itemImage.sprite = _itemImage.sprite;
        SetColor(1);
    }

    public void SetColor(float _alpha)
    {
        Color color = itemImage.color;
        color.a = _alpha;
        itemImage.color = color;
    }
}
