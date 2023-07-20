using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/*
[ 플레이 Scene 스크립트 ]
1. GameScene이 로드되면 GameScene.cs에 의해서 생성되어 사용된다.
2. UI_PlayScene.cs은 모든 Popup을 생성하여 저장한다.
3. 실시간으로 플레이어 정보, 미니맵, 스킬/소비아이템 슬롯바, 퀘스트 알림의 UI를 보여준다.
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
    public UI_MenuPopup _menu;              // 일시정시 메뉴

    public List<UI_UseItemSlot> UseItemBarList;

    public override bool Init()
	{
		if (base.Init() == false)
			return false;

        UseItemBarList = new List<UI_UseItemSlot>();

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
            Managers.Game.GetPlayer().GetComponent<PlayerController>().LevelUpEffect();
            Managers.UI.ShowPopupUI<UI_GuidePopup>().SetInfo($"({Managers.Game.Level})\n레벨이 올랐습니다!! \n\n\n\n\n\n\n\n\n", Color.yellow);
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

            UseItemBarList.Add(slot);
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
        if (Managers.Game.currentMonster.IsNull() == false)
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

    // 소비 아이템 사용
    public void UsingItem(int key)
    {
        for(int i=0; i<UseItemBarList.Count; i++)
        {
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
