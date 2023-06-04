using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;

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

    Action _onClickYesButton;
    UI_InvenItem _invenItem;
    public void RefreshUI(UI_InvenItem invenItem, Action onClickYesButton)
    {
        _onClickYesButton = onClickYesButton;
        _invenItem = invenItem;

        itemCount = 1;
        itemMaxCount = invenItem.itemCount;
        
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
