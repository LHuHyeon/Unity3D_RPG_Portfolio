using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_InvenPopup : UI_Popup
{
    /*
    1. 타이틀 잡으면 ui 이동 가능 (해상도 밖으로는 못나감.)
    2. 인벤 슬롯 초기화
    3. 인벤토리 관련 여기서 모두 관리.
    4. 스크롤 뷰 마우스 휠 속도 올리기
    */
    
    enum Gameobjects
    {
        Content,
        Title,
        ExitButton,
    }

    enum Texts
    {
        GoldText,
    }

    [SerializeField]
    int invenCount = 42;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;
    
        BindObject(typeof(Gameobjects));
        BindText(typeof(Texts));

        SetInfo();

        gameObject.SetActive(false);

        return true;
    }

    public void SetInfo()
    {
        ResetSlot();
        SetEventHandler();
    }

    public void AcquireItem(ItemData item, int count = 1)
    {
        foreach(UI_InvenItem slot in Managers.Game.InvenSlots)
        {
            // 아이템이 없으면 넣기
            if (slot.item == null)
            {
                slot.AddItem(item, count);
                break;
            }

            if (item is UseItemData)
            {
                // 같은 아이템이 존재하면 개수 추가
                if (item.id == slot.item.id)
                {
                    slot.SetCount(count);
                    break;
                }
            }
        }
    }

    void ResetSlot()
    {
        GameObject grid = GetObject((int)Gameobjects.Content);
        Managers.Game.InvenSlots = new List<UI_InvenItem>();

        foreach(Transform child in grid.transform)
            Managers.Resource.Destroy(child.gameObject);

        for(int i=0; i<invenCount; i++)
            Managers.Game.InvenSlots.Add(Managers.UI.MakeSubItem<UI_InvenItem>(parent: grid.transform));
    }

    void SetEventHandler()
    {

    }

    void RefreshUI()
    {
        GetText((int)Texts.GoldText).text = Managers.Game.Gold.ToString();
    }
}
