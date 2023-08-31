using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/*
 * File :   UI_InvenPopup.cs
 * Desc :   인벤토리 관리 Popup 
 *
 & Functions
 &  [Public]
 &  : Init()            - 초기 설정
 &  : IsInvenMaxSize()  - 인벤토리 여유 공간 확인
 &  : AcquireItem()     - 인벤토리 슬롯에 아이템 저장
 &  : ResetPos()        - Popup 위치 초기화
 &
 &  [Private]
 &  : OnInventoryUI()   - 인벤토리 활성화or비활성화
 &  : SetInfo()         - 기능 설정
 &  : ResetSlot()       - 슬롯 초기화
 &  : SetEventHandler() - EventHandler 설정
 &  : RefreshUI()       - 새로고침 UI
 &  : Exit()            - 나가기
 *
 */

public class UI_InvenPopup : UI_Popup
{
    enum Gameobjects
    {
        Background,
        Content,
        Title,
        ExitButton,
    }

    enum Texts
    {
        GoldText,
    }

    private List<UI_InvenItem>  invenSlots;         // 슬롯 List

    [SerializeField]
    private int                 invenCount = 42;    // 인벤 슬롯 개수

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        invenSlots = new List<UI_InvenItem>();
        popupType = Define.Popup.Inventory;

        // 자식 객체 불러오기
        BindObject(typeof(Gameobjects));
        BindText(typeof(Texts));

        // InputManager에 입력 등록
        Managers.Input.KeyAction -= OnInventoryUI;
        Managers.Input.KeyAction += OnInventoryUI;

        SetInfo();

        Invoke("DelayInit", 0.0001f);

        return true;
    }
    void DelayInit() { RefreshUI(); Managers.UI.ClosePopupUI(this); }

    void Update()
    {
        // 인벤토리 활성화되면 실시간 새로고침
        if (Managers.Game.isPopups[Define.Popup.Inventory] == true)
            RefreshUI();
    }

    // 인벤토리 자리 확인
    public bool IsInvenMaxSize()
    {
        foreach(UI_InvenItem slot in invenSlots)
        {
            if (slot.item.IsNull() == true)
                return false;
        }

        return true;
    }

    // 인벤토리 슬롯 아이템 저장
    public bool AcquireItem(ItemData item, int count = 1)
    {
        // 모든 슬롯 확인
        foreach(UI_InvenItem slot in invenSlots)
        {
            // 슬롯에 아이템이 없으면
            if (slot.item.IsNull() == true)
            {
                // 아이템 저장
                slot.AddItem(item, count);
                return true;
            }

            // 소비 아이템이라면
            if (item is UseItemData)
            {
                // 아이템의 id가 같다면 똑같은 아이템이므로
                if (item.id == slot.item.id)
                {
                    // 개수 추가
                    slot.SetCount(count);
                    return true;
                }
            }
        }
        
        // 경고문 생성
        Managers.UI.MakeSubItem<UI_Guide>().SetInfo("인벤토리가 가득 찼습니다.", Color.red);

        return false;
    }

    // 인벤토리 Popup 초기화
    public void ResetPos()
    {
        RectTransform invenPos = GetObject((int)Gameobjects.Background).GetComponent<RectTransform>();
        invenPos.anchoredPosition = new Vector2(330, 0);
    }

    // 인벤토리 활성화
    private void OnInventoryUI()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            Managers.Game.isPopups[Define.Popup.Inventory] = !Managers.Game.isPopups[Define.Popup.Inventory];

            // 인벤토리 Popup On/Off
            if (Managers.Game.isPopups[Define.Popup.Inventory])
                Managers.UI.OnPopupUI(this);
            else
                Exit();
        }
    }

    // 기능 설정
    private void SetInfo()
    {
        ResetSlot();        // 슬롯 초기화
        SetEventHandler();  // EventHandler 설정
    }

    private void ResetSlot()
    {
        // 슬롯들을 담고 있는 부모 가져오기
        GameObject grid = GetObject((int)Gameobjects.Content);

        // 슬롯 모두 삭제
        foreach(Transform child in grid.transform)
            Managers.Resource.Destroy(child.gameObject);

        // invenCount만큼 슬롯 생성
        for(int i=0; i<invenCount; i++)
        {
            // 슬롯 생성
            UI_InvenItem invenItem = Managers.UI.MakeSubItem<UI_InvenItem>(parent: grid.transform);

            // 슬롯 위치 번호
            invenItem.invenNumber = i;

            // 위치 번호가 세이브에 있다면 item 가져오기
            if (Managers.Game.InvenItem.TryGetValue(i, out ItemData item) == true)
            {
                // 기능 설정
                invenItem.SetInfo();

                // 소비 아이템이라면
                if (item is UseItemData)
                {
                    // 아이템 개수와 함께 저장
                    UseItemData useItem = item as UseItemData;
                    invenItem.AddItem(useItem, useItem.itemCount);
                }
                else
                    invenItem.AddItem(item);    // 그냥 item 저장
            }

            // 생성된 슬롯 List에 저장
            invenSlots.Add(invenItem);
        }
    }

    private void SetEventHandler()
    {
        // Title 잡고 인벤토리 이동
        RectTransform invenPos = GetObject((int)Gameobjects.Background).GetComponent<RectTransform>();
        GetObject((int)Gameobjects.Title).BindEvent((PointerEventData eventData)=>
        {
            // 상호작용 중이면 이동X
            if (Managers.Game.IsInteract == true)
                return;

            invenPos.anchoredPosition = new Vector2
            (
                Mathf.Clamp(invenPos.anchoredPosition.x + eventData.delta.x, -655, 655),
                Mathf.Clamp(invenPos.anchoredPosition.y + eventData.delta.y, -253, 217)
            );
        }, Define.UIEvent.Drag);

        // Order 설정
        GetObject((int)Gameobjects.Background).BindEvent((PointerEventData eventData)=>
        {
            Managers.UI.SetOrder(GetComponent<Canvas>());
        }, Define.UIEvent.Click);

        // Exit 버튼
        GetObject((int)Gameobjects.ExitButton).BindEvent((PointerEventData eventData)=>
        {
            if (Managers.Game.IsInteract == true)
                return;
                
            Exit();
        }, Define.UIEvent.Click);
    }

    private void RefreshUI()
    {   
        // 골드 개수 불러오기
        GetText((int)Texts.GoldText).text = Managers.Game.Gold.ToString();
    }

    private void Exit()
    {
        Managers.Game._playScene._slotTip.OnSlotTip(false);
        Managers.UI.ClosePopupUI(this);
    }
}
