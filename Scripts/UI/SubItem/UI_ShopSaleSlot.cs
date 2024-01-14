using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * File :   UI_ShopSaleSlot.cs
 * Desc :   UI_ShopPopup.cs에서 생성되며 아이템 판매가 등록됐을 때 기능
 *
 & Functions
 &  [Public]
 &  : Init()                - 초기 설정
 &  : SetInfo()             - 기능 설정
 &  : GetSale()             - 판매 진행
 &  : Clear()               - 초기화
 &
 &  [Prviate]
 &  : OnClickCloseButton()  - 판매 등록 취소
 *
 */

public class UI_ShopSaleSlot : UI_Base
{
    enum Buttons
    {
        CloseButton,
    }

    enum Images
    {
        SaleItemIcon,
    }

    enum Texts
    {
        SaleItemCountText,
    }

    private UI_InvenSlot    _invenItem;             // 인벤토리 슬롯
    private Image           _icon;

    private int             _saleItemCount = 0;     // 판매될 개수
    private string          _itemCountText;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindButton(typeof(Buttons));
        BindImage(typeof(Images));
        BindText(typeof(Texts));

        GetButton((int)Buttons.CloseButton).onClick.AddListener(OnClickCloseButton);

        GetImage((int)Images.SaleItemIcon).sprite = _icon.sprite;
        GetText((int)Texts.SaleItemCountText).text = _itemCountText;

        return true;
    }

    public void SetInfo(UI_InvenSlot invenItem, int subItemCount = 1)
    {
        _invenItem = invenItem;
        _saleItemCount = subItemCount;
        _icon = _invenItem.icon;

        // 판매할 인벤토리의 슬롯 잠그기
        _invenItem.IsLock = true;

        // 소비 아이템이면 개수 활성화
        if (invenItem.item is UseItemData)
            _itemCountText = _saleItemCount.ToString();
        else
            _itemCountText = "";
    }

    // 판매 진행
    public void GetSale()
    {
        // 장비면 강화 확인 후 판매
        if ((_invenItem.item is EquipmentData) == true)
        {
            EquipmentData equipment = _invenItem.item as EquipmentData;
            Managers.Game.Gold += _invenItem.item.itemPrice + (int)((equipment.itemPrice / 4) * (equipment.upgradeCount));
        }
        else
            Managers.Game.Gold += _invenItem.item.itemPrice * _saleItemCount;

        // 판매된 슬롯에 개수 차감
        _invenItem.SetCount(-_saleItemCount);

        Clear();
    }

    // 판매 등록 취소
    private void OnClickCloseButton()
    {
        Managers.Game._playScene._shop.saleList.Remove(this);

        Clear();
    }

    public void Clear()
    {
        _invenItem.IsLock = false;

        Managers.Resource.Destroy(this.gameObject);
    }
}
