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
        Background,
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

        Managers.Input.KeyAction -= OnInventory;
        Managers.Input.KeyAction += OnInventory;

        SetInfo();

        GetObject((int)Gameobjects.Background).SetActive(false);

        return true;
    }

    public void SetInfo()
    {
        ResetSlot();
        SetEventHandler();
    }
    
    void OnInventory()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            Managers.Game.isInventory = !Managers.Game.isInventory;

            GetObject((int)Gameobjects.Background).SetActive(Managers.Game.isInventory);
        }
    }

    void ResetSlot()
    {
        GameObject grid = GetObject((int)Gameobjects.Content);
        Managers.Game.InvenSlots = new List<UI_InvenItem>();

        foreach(Transform child in grid.transform)
            Managers.Resource.Destroy(child.gameObject);

        for(int i=0; i<invenCount; i++)
            Managers.Game.InvenSlots.Add(Managers.UI.MakeSubItem<UI_InvenItem>(parent: grid.transform, name: "Slot"));
    }

    void SetEventHandler()
    {

    }

    void RefreshUI()
    {
        GetText((int)Texts.GoldText).text = Managers.Game.Gold.ToString();
    }
}
