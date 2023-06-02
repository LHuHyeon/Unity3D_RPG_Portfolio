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

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindButton(typeof(Buttons));
        BindText(typeof(Texts));

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

        if (numberSlider == null)
            Debug.Log("Slider Null");
        
        numberSlider.maxValue = itemMaxCount;
        numberSlider.value = itemCount;

        if (GetText((int)Texts.NumberCountText) == null)
            Debug.Log("Text null");

        GetText((int)Texts.NumberCountText).text = itemCount.ToString();
    }

    void OnClickMinusButton()
    {
        itemCount--;
        GetText((int)Texts.NumberCountText).text = itemCount.ToString();
    }

    void OnClickPlusButton()
    {
        itemCount++;
        GetText((int)Texts.NumberCountText).text = itemCount.ToString();
    }

    void OnClickYesButton()
    {
        Managers.UI.ClosePopupUI(this);

        _invenItem.SetCount(-itemCount);

        if (_onClickYesButton != null)
            _onClickYesButton.Invoke();
    }

    void OnClickNoButton()
    {
        Managers.UI.ClosePopupUI(this);
    }
}
