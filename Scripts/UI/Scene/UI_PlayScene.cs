using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/*
 * File :   UI_PlayScene.cs
 * Desc :   게임 플레이 Scene UI
 *
 & Functions
 &  [Public]
 &  : Init()                - 초기 설정
 &  : IsMiniMap()           - 미니맵 활성화 여부
 &  : RefreshUI()           - 새로고침 UI
 &  : SetQuestNoticeBar()   - Scene UI 퀘스트 알림 추가
 &  : UsingItem()           - 퀵슬롯 아이템 사용
 &  : OnMonsterBar()        - 몬스터바 활성화
 &  : CloseMonsterBar()     - 몬스터바 비활성화
 &
 &  [Private]
 &  : RefreshStat()         - 스탯 새로고침
 &  : SetRatio()            - Slider NaN 방지용
 &  : SetInfo()             - 기능 설정
 *
 */

public class UI_PlayScene : UI_Scene
{
    enum Gameobjects
    {
        SkillBar,
        ItemBar,
        ultSkillSlot,
        QuestListBar,
        MonsterBar,
        MiniMap,
    }

    enum Images
    {
        PlayerIcon,
    }

    enum Texts
    {
        LevelBarText,
        NameBarText,
        HpBarText,
        MpBarText,
        ExpBarText,
        MonsterHpBarText,
        MonsterNameBarText,
    }

    enum Sliders
    {
        HpBar,
        MpBar,
        ExpBar,
        MonsterHpBar,
    }

    enum Buttons
    {
        LevelUpButton,
        AddGoldButton,
    }

    public UI_InvenPopup        _inventory;     // 인벤토리 
    public UI_EqStatPopup       _equipment;     // 장비/스탯 
    public UI_SkillPopup        _skill;         // 스킬 
    public UI_SlotTipPopup      _slotTip;       // 슬롯팁
    public UI_ShopPopup         _shop;          // 상점
    public UI_TalkPopup         _talk;          // 대화
    public UI_QuestPopup        _quest;         // 퀘스트
    public UI_UpgradePopup      _upgrade;       // 강화
    public UI_MenuPopup         _menu;          // 일시정시 메뉴

    public List<UI_UseItemSlot> UseItemBarList; // 아이템 퀵슬롯 바 List

    public override bool Init()
	{
		if (base.Init() == false)
			return false;

        UseItemBarList = new List<UI_UseItemSlot>();

        // 자식 객체 불러오기
        BindObject(typeof(Gameobjects));
        BindImage(typeof(Images));
        BindButton(typeof(Buttons));
        BindText(typeof(Texts));
        Bind<Slider>(typeof(Sliders));

        SetInfo();

        // 팝업 초기화
        Managers.UI.CloseAllPopupUI();

        // 기능 팝업 생성
        _inventory = Managers.UI.ShowPopupUI<UI_InvenPopup>();
        _equipment = Managers.UI.ShowPopupUI<UI_EqStatPopup>();
        _skill = Managers.UI.ShowPopupUI<UI_SkillPopup>();
        _slotTip = Managers.UI.ShowPopupUI<UI_SlotTipPopup>();
        _shop = Managers.UI.ShowPopupUI<UI_ShopPopup>();
        _talk = Managers.UI.ShowPopupUI<UI_TalkPopup>();
        _quest = Managers.UI.ShowPopupUI<UI_QuestPopup>();
        _upgrade = Managers.UI.ShowPopupUI<UI_UpgradePopup>();
        _menu = Managers.UI.ShowPopupUI<UI_MenuPopup>();
        DontDestroyOnLoad(Managers.Resource.Instantiate($"UI/SubItem/UI_DragSlot"));

        // 세이브를 불러온게 아니라면
        if (Managers.Game.isSaveLoad == false)
        {
            // 기본 장비 장착
            Managers.Game.CurrentWeapon = Managers.Data.CallItem(2001) as WeaponItemData;
            Managers.Game.CurrentArmor.Add(Define.ArmorType.Chest, Managers.Data.CallItem(2) as ArmorItemData);
            Managers.Game.CurrentArmor.Add(Define.ArmorType.Pants, Managers.Data.CallItem(3) as ArmorItemData);
        }

		return true;
	}

    void Update()
    {
        RefreshStat();
    }

    // 미니맵 활성화 여부 (던전에선 끄기)
    public void IsMiniMap(bool isActive) { GetObject((int)Gameobjects.MiniMap).SetActive(isActive); }

    public void RefreshUI()
    {
        // 레벨 7 이상이면 궁극기 슬롯 오픈
        if (Managers.Game.Level >= 5)
            GetObject((int)Gameobjects.ultSkillSlot).SetActive(true);

        RefreshStat();
    }

    // 씬에 퀘스트 알림 추가
    public UI_QuestNoticeSlot SetQuestNoticeBar(QuestData quest)
    {
        UI_QuestNoticeSlot sceneQuestSlot = Managers.UI.MakeSubItem<UI_QuestNoticeSlot>(parent: GetObject((int)Gameobjects.QuestListBar).transform);
        sceneQuestSlot.SetInfo(quest);
        return sceneQuestSlot;
    }

    // 소비 아이템 사용
    public void UsingItem(int key)
    {
        // 소비아이템 퀵슬롯 List 확인
        for(int i=0; i<UseItemBarList.Count; i++)
        {
            // 키가 같으면 사용
            if (key == UseItemBarList[i].key)
            {
                UseItemData useItem = UseItemBarList[i].item as UseItemData;
                if (useItem.UseItem(useItem) == true)
                {
                    UseItemBarList[i].SetCount(-1);
                    return;
                }
            }
        }

        Debug.Log("장착된 소비 아이템이 없습니다.");
    }

    // 몬스터 정보 상단 활성화
    public void OnMonsterBar(MonsterStat monsterStat)
    {
        if (GetObject((int)Gameobjects.MonsterBar).activeSelf == false)
            GetObject((int)Gameobjects.MonsterBar).SetActive(true);

        Managers.Game.currentMonster = monsterStat;
    }

    public void CloseMonsterBar() 
    { 
        Managers.Game.currentMonster = null;
        GetObject((int)Gameobjects.MonsterBar).SetActive(false); 
    }

    private void RefreshStat()
    {
        // 스탯 text 설정
        GetText((int)Texts.HpBarText).text = Managers.Game.Hp + " / " + Managers.Game.MaxHp;
        GetText((int)Texts.MpBarText).text = Managers.Game.Mp + " / " + Managers.Game.MaxMp;
        GetText((int)Texts.ExpBarText).text = Managers.Game.Exp + " / " + Managers.Game.TotalExp;
        GetText((int)Texts.LevelBarText).text = Managers.Game.Level.ToString();

        // Slider 설정
        SetRatio(Get<Slider>((int)Sliders.HpBar), (float)Managers.Game.Hp / Managers.Game.MaxHp);
        SetRatio(Get<Slider>((int)Sliders.MpBar), (float)Managers.Game.Mp / Managers.Game.MaxMp);
        SetRatio(Get<Slider>((int)Sliders.ExpBar), (float)Managers.Game.Exp / Managers.Game.TotalExp);

        // 몬스터랑 전투 중일 때
        if (Managers.Game.currentMonster.IsNull() == false)
        {
            GetText((int)Texts.MonsterNameBarText).text = Managers.Game.currentMonster.Name;
            GetText((int)Texts.MonsterHpBarText).text = Managers.Game.currentMonster.Hp + " / " + Managers.Game.currentMonster.MaxHp;
            SetRatio(Get<Slider>((int)Sliders.MonsterHpBar), (float)Managers.Game.currentMonster.Hp / Managers.Game.currentMonster.MaxHp);
        }
    }

    // Slider NaN 방지
    private void SetRatio(Slider slider, float ratio)
    {
        if (float.IsNaN(ratio) == true)
            slider.value = 0;
        else
            slider.value = ratio;
    }

    private void SetInfo()
    {
        GetText((int)Texts.NameBarText).text = Managers.Game.Name;

        // 소비 아이템 퀵슬롯 초기화
        foreach(Transform child in GetObject((int)Gameobjects.ItemBar).transform)
            Managers.Resource.Destroy(child.gameObject);

        // 소비 아이템 키 슬롯
        for(int i=1; i<=2; i++)
        {
            UI_UseItemSlot slot = Managers.UI.MakeSubItem<UI_UseItemSlot>(GetObject((int)Gameobjects.ItemBar).transform);
            slot.key = i;

            UseItemBarList.Add(slot);
        }

        GetObject((int)Gameobjects.ultSkillSlot).SetActive(false);
        CloseMonsterBar();

        // 퀘스트 알림 초기화
        foreach(Transform child in GetObject((int)Gameobjects.QuestListBar).transform)
            Managers.Resource.Destroy(child.gameObject);

        RefreshUI();
    }
}
