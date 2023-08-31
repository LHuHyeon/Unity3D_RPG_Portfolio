using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;

/*
 * File :   UI_NumberCheckPopup.cs
 * Desc :   개수 확인 Popup ( 현재 상점에서 사용 중 )
 *
 & Functions
 &  [Public]
 &  : Init()    - 초기 설정
 &  : SetInfo() - 기능 설정
 &
 &  [Private]
 &  : OnClickMinusButton()  - 마이너스 버튼
 &  : OnClickPlusButton()   - 플러스 버튼
 &  : OnClickYesButton()    - 확인 버튼
 &  : OnClickNoButton()     - 취소 버튼
 &  : RefreshUI()           - 새로고침 UI
 *
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

    private int             itemCount = 0;      // 현재 개수
    private int             itemMaxCount = 0;   // 최대 개수

    private Action<int>     _onClickYesButton;  // 확인 버튼 누를 시 호출
    private UI_InvenItem    _invenItem;         // 인벤토리 슬롯

    [SerializeField]
    private Slider          numberSlider;

    [SerializeField]
    private TextMeshProUGUI _itemCountText;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        // 자식 객체 불러오기
        BindObject(typeof(Gameobjects));
        BindButton(typeof(Buttons));

        // 버튼 기능 등록
        GetButton((int)Buttons.MinusButton).onClick.AddListener(OnClickMinusButton);
        GetButton((int)Buttons.PlusButton).onClick.AddListener(OnClickPlusButton);
        GetButton((int)Buttons.NoButton).onClick.AddListener(OnClickNoButton);
        GetButton((int)Buttons.YesButton).onClick.AddListener(OnClickYesButton);

        // Order 설정
        GetObject((int)Gameobjects.Background).BindEvent((PointerEventData eventData)=>
        {
            Managers.UI.SetOrder(GetComponent<Canvas>());
        }, Define.UIEvent.Click);
        
        // 슬라이더 사용 시 기능 등록
        numberSlider.onValueChanged.AddListener((float value)=>
        {
            itemCount = (int)value;
            _itemCountText.text = itemCount.ToString();
        });

        return true;
    }

    // 인벤토리 받으며 세팅 (판매할 때 사용 중)
    public void SetInfo(UI_InvenItem invenItem, Action<int> onClickYesButton)
    {
        _onClickYesButton = onClickYesButton;
        _invenItem = invenItem;

        itemMaxCount = invenItem.itemCount;

        RefreshUI();
    }
    
    // 아이템 받으며 세팅 (구매할 때 사용 중)
    public void SetInfo(ItemData item, Action<int> onClickYesButton)
    {
        _onClickYesButton = onClickYesButton;

        itemMaxCount = (int)(Managers.Game.Gold / item.itemPrice);

        RefreshUI();
    }

    // 마이너스 버튼
    private void OnClickMinusButton()
    {
        itemCount = Mathf.Clamp(--itemCount, 1, itemMaxCount);
        numberSlider.value = itemCount;
        _itemCountText.text = itemCount.ToString();
    }

    // 플러스 버튼
    private void OnClickPlusButton()
    {
        itemCount = Mathf.Clamp(++itemCount, 1, itemMaxCount);
        numberSlider.value = itemCount;
        _itemCountText.text = itemCount.ToString();
    }

    // 확인 버튼
    private void OnClickYesButton()
    {
        Managers.UI.ClosePopupUI(this);

        if (_onClickYesButton.IsNull() == false)
            _onClickYesButton.Invoke(itemCount);
    }

    // 취소 버튼
    private void OnClickNoButton()
    {
        Managers.UI.ClosePopupUI(this);
    }

    private void RefreshUI()
    {
        Managers.UI.SetOrder(GetComponent<Canvas>());

        itemCount = 1;

        numberSlider.minValue = itemCount;
        numberSlider.maxValue = itemMaxCount;
        numberSlider.value = itemCount;

        _itemCountText.text = itemCount.ToString();
    }
}
