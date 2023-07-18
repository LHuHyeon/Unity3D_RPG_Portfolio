using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;

/*
[ 개수 체크 Popup 스크립트 ]
1. 아이템을 구매or판매할 때 사용된다.
2. 자주 호출되는 함수 : RefreshUI()
*/

public class UI_NumberCheckPopup : UI_Popup
{   
    enum Gameobjects
    {
        Background,
    }

    enum Buttons
    {
        MinusButton,
        PlusButton,
        NoButton,
        YesButton,
    }

    [SerializeField]
    Slider numberSlider;

    [SerializeField]
    TextMeshProUGUI _itemCountText;

    int itemCount = 0;
    int itemMaxCount = 0;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindObject(typeof(Gameobjects));
        BindButton(typeof(Buttons));

        SetInfo();

        return true;
    }

    void SetInfo()
    {
        // Order 설정
        GetObject((int)Gameobjects.Background).BindEvent((PointerEventData eventData)=>
        {
            Managers.UI.SetOrder(GetComponent<Canvas>());
        }, Define.UIEvent.Click);

        GetButton((int)Buttons.MinusButton).onClick.AddListener(OnClickMinusButton);
        GetButton((int)Buttons.PlusButton).onClick.AddListener(OnClickPlusButton);
        GetButton((int)Buttons.NoButton).onClick.AddListener(OnClickNoButton);
        GetButton((int)Buttons.YesButton).onClick.AddListener(OnClickYesButton);
        numberSlider.onValueChanged.AddListener((float value)=>
        {
            itemCount = (int)value;
            _itemCountText.text = itemCount.ToString();
        });
    }

    Action<int> _onClickYesButton;
    UI_InvenItem _invenItem;
    // 판매할 때 사용 중
    public void RefreshUI(UI_InvenItem invenItem, Action<int> onClickYesButton)
    {
        _onClickYesButton = onClickYesButton;
        _invenItem = invenItem;

        itemMaxCount = invenItem.itemCount;

        RefreshUI();
    }
    
    // 구매할 때 사용 중
    public void RefreshUI(ItemData item, Action<int> onClickYesButton)
    {
        _onClickYesButton = onClickYesButton;

        itemMaxCount = (int)(Managers.Game.Gold / item.itemPrice);

        RefreshUI();
    }

    void RefreshUI()
    {
        Managers.UI.SetOrder(GetComponent<Canvas>());

        itemCount = 1;

        numberSlider.minValue = itemCount;
        numberSlider.maxValue = itemMaxCount;
        numberSlider.value = itemCount;

        _itemCountText.text = itemCount.ToString();
    }

    void OnClickMinusButton()
    {
        itemCount = Mathf.Clamp(--itemCount, 1, itemMaxCount);
        numberSlider.value = itemCount;
        _itemCountText.text = itemCount.ToString();
    }

    void OnClickPlusButton()
    {
        itemCount = Mathf.Clamp(++itemCount, 1, itemMaxCount);
        numberSlider.value = itemCount;
        _itemCountText.text = itemCount.ToString();
    }

    void OnClickYesButton()
    {
        Managers.UI.ClosePopupUI(this);

        if (_onClickYesButton.IsNull() == false)
            _onClickYesButton.Invoke(itemCount);
    }

    void OnClickNoButton()
    {
        Managers.UI.ClosePopupUI(this);
    }
}
