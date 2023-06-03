using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_NumberCheckPopup : UI_Popup
{
    enum Buttons
    {
        MinusButton,
        PlusButton,
        NoButton,
        YesButton,
    }

    enum Texts
    {
        NumberCountText,
    }

    [SerializeField]
    Slider numberSlider;

    int itemCount = 0;
    int itemMaxCount = 0;

    string _itemCounttext;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindButton(typeof(Buttons));
        BindText(typeof(Texts));

        GetText((int)Texts.NumberCountText).text = _itemCounttext;

        SetInfo();

        return true;
    }

    void SetInfo()
    {
        GetButton((int)Buttons.MinusButton).onClick.AddListener(OnClickMinusButton);
        GetButton((int)Buttons.PlusButton).onClick.AddListener(OnClickPlusButton);
        GetButton((int)Buttons.NoButton).onClick.AddListener(OnClickNoButton);
        GetButton((int)Buttons.YesButton).onClick.AddListener(OnClickYesButton);
    }

    Action _onClickYesButton;
    UI_InvenItem _invenItem;
    public void RefreshUI(UI_InvenItem invenItem, Action onClickYesButton)
    {
        _onClickYesButton = onClickYesButton;
        _invenItem = invenItem;

        itemCount = 1;
        itemMaxCount = invenItem.itemCount;
        
        numberSlider.maxValue = itemMaxCount;
        numberSlider.value = itemCount;

        _itemCounttext = itemCount.ToString();
    }

    void OnClickMinusButton()
    {
        Mathf.Clamp(--itemCount, 1, itemMaxCount);
        _itemCounttext = itemCount.ToString();
    }

    void OnClickPlusButton()
    {
        Mathf.Clamp(++itemCount, 1, itemMaxCount);
        _itemCounttext = itemCount.ToString();
    }

    void OnClickYesButton()
    {
        Managers.UI.ClosePopupUI(this);

        // 인벤에 차감개수 알려주기
        _invenItem.subItemCount = itemCount;

        if (_onClickYesButton != null)
            _onClickYesButton.Invoke();
    }

    void OnClickNoButton()
    {
        Managers.UI.ClosePopupUI(this);
    }
}
