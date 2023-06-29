using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_PlayScene : UI_Scene
{
    enum Gameobjects
    {
        SkillBar,
        ItemBar,
        ultSkillSlot,
        QuestListBar,
        MonsterBar,
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
        SpawnButton,
        LevelUpButton,
        AddGoldButton,
    }

    public UI_InvenPopup _inventory;        // 인벤토리 
    public UI_EqStatPopup _equipment;       // 장비/스탯 
    public UI_SkillPopup _skill;            // 스킬 
    public UI_SlotTipPopup _slotTip;        // 슬롯팁
    public UI_ShopPopup _shop;              // 상점
    public UI_TalkPopup _talk;              // 대화
    public UI_QuestPopup _quest;            // 퀘스트
    public UI_UpgradePopup _upgrade;        // 강화

    public override bool Init()
	{
		if (base.Init() == false)
			return false;

        BindObject(typeof(Gameobjects));
        BindImage(typeof(Images));
        BindButton(typeof(Buttons));
        BindText(typeof(Texts));
        Bind<Slider>(typeof(Sliders));

        SetInfo();

        _inventory = Managers.UI.ShowPopupUI<UI_InvenPopup>();
        _equipment = Managers.UI.ShowPopupUI<UI_EqStatPopup>();
        _skill = Managers.UI.ShowPopupUI<UI_SkillPopup>();
        _slotTip = Managers.UI.ShowPopupUI<UI_SlotTipPopup>();
        _shop = Managers.UI.ShowPopupUI<UI_ShopPopup>();
        _talk = Managers.UI.ShowPopupUI<UI_TalkPopup>();
        _quest = Managers.UI.ShowPopupUI<UI_QuestPopup>();
        _upgrade = Managers.UI.ShowPopupUI<UI_UpgradePopup>();
        DontDestroyOnLoad(Managers.Resource.Instantiate($"UI/SubItem/UI_DragSlot"));

        // 장비 미리 장착
        Managers.Game.CurrentWeapon = Managers.Data.CallItem(2001) as WeaponItemData;
        Managers.Game.CurrentArmor.Add(Define.ArmorType.Helm, Managers.Data.CallItem(3001) as ArmorItemData);
        Managers.Game.CurrentArmor.Add(Define.ArmorType.Chest, Managers.Data.CallItem(3005) as ArmorItemData);
        Managers.Game.CurrentArmor.Add(Define.ArmorType.Pants, Managers.Data.CallItem(3009) as ArmorItemData);
        Managers.Game.CurrentArmor.Add(Define.ArmorType.Boots, Managers.Data.CallItem(3013) as ArmorItemData);
        Managers.Game.CurrentArmor.Add(Define.ArmorType.Gloves, Managers.Data.CallItem(3017) as ArmorItemData);

		return true;
	}

    void Update()
    {
        RefreshStat();
    }

    public void SetInfo()
    {
        GetText((int)Texts.NameBarText).text = Managers.Game.Name;

        // -- TODO : Test 용 코드 나중에 삭제하기
        GetButton((int)Buttons.SpawnButton).onClick.AddListener(()=>
        {
            GameObject obj = Managers.Game.Spawn(Define.WorldObject.Monster, "Monster/Skeleton1");
            obj.transform.position += new Vector3(0, 0, 5f);
        });

        GetButton((int)Buttons.LevelUpButton).onClick.AddListener(()=>
        {
            // 다음 레벨 확인
            if (Managers.Data.Level.ContainsKey(Managers.Game.Level + 1) == false)
            {
                Debug.Log("만렙 입니다!");
                return;
            }

            Managers.Game.RefreshStat(++Managers.Game.Level);
        });

        GetButton((int)Buttons.AddGoldButton).onClick.AddListener(()=>{ Managers.Game.Gold += 100; });

        // --

        foreach(Transform child in GetObject((int)Gameobjects.ItemBar).transform)
            Managers.Resource.Destroy(child.gameObject);

        // 소비 아이템 키 슬롯
        for(int i=1; i<=2; i++)
        {
            UI_UseItemSlot slot = Managers.UI.MakeSubItem<UI_UseItemSlot>(GetObject((int)Gameobjects.ItemBar).transform);
            slot.key = i;
            slot.keyText.text = i.ToString();
            slot.itemCountText.text = "";

            Managers.Game.UseItemBarList.Add(i, slot);
        }

        GetObject((int)Gameobjects.ultSkillSlot).SetActive(false);
        ClostMonsterBar();

        foreach(Transform child in GetObject((int)Gameobjects.QuestListBar).transform)
            Managers.Resource.Destroy(child.gameObject);

        RefreshUI();
    }

    public void RefreshUI()
    {
        // 레벨 10 이상이면 궁극기 슬롯 오픈
        if (Managers.Game.Level >= 10)
            GetObject((int)Gameobjects.ultSkillSlot).SetActive(true);

        RefreshStat();
    }

    public void RefreshStat()
    {
        GetText((int)Texts.HpBarText).text = Managers.Game.Hp + " / " + Managers.Game.MaxHp;
        GetText((int)Texts.MpBarText).text = Managers.Game.Mp + " / " + Managers.Game.MaxMp;
        GetText((int)Texts.ExpBarText).text = Managers.Game.Exp + " / " + Managers.Game.TotalExp;
        GetText((int)Texts.LevelBarText).text = Managers.Game.Level.ToString();

        SetRatio(Get<Slider>((int)Sliders.HpBar), (float)Managers.Game.Hp / Managers.Game.MaxHp);
        SetRatio(Get<Slider>((int)Sliders.MpBar), (float)Managers.Game.Mp / Managers.Game.MaxMp);
        SetRatio(Get<Slider>((int)Sliders.ExpBar), (float)Managers.Game.Exp / Managers.Game.TotalExp);

        // 몬스터랑 전투 중일 때
        if (Managers.Game.currentMonster != null)
        {
            GetText((int)Texts.MonsterNameBarText).text = Managers.Game.currentMonster.Name;
            GetText((int)Texts.MonsterHpBarText).text = Managers.Game.currentMonster.Hp + " / " + Managers.Game.currentMonster.MaxHp;
            SetRatio(Get<Slider>((int)Sliders.MonsterHpBar), (float)Managers.Game.currentMonster.Hp / Managers.Game.currentMonster.MaxHp);
        }
    }

    // 씬에 퀘스트 알림 추가
    public UI_QuestNoticeSlot SetQuestNoticeBar(QuestData quest)
    {
        UI_QuestNoticeSlot sceneQuestSlot = Managers.UI.MakeSubItem<UI_QuestNoticeSlot>(parent: GetObject((int)Gameobjects.QuestListBar).transform);
        sceneQuestSlot.SetInfo(quest);
        return sceneQuestSlot;
    }

    public void SetRatio(Slider slider, float ratio)
    {
        if (float.IsNaN(ratio) == true)
            slider.value = 0;
        else
            slider.value = ratio;
    }

    // 몬스터 정보 상단 활성화
    public void OnMonsterBar(MonsterStat monsterStat)
    {
        if (GetObject((int)Gameobjects.MonsterBar).activeSelf == false)
            GetObject((int)Gameobjects.MonsterBar).SetActive(true);

        Managers.Game.currentMonster = monsterStat;
    }

    public void ClostMonsterBar() 
    { 
        Managers.Game.currentMonster = null;
        GetObject((int)Gameobjects.MonsterBar).SetActive(false); 
    }
}
