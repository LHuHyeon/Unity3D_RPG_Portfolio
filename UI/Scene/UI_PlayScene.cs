using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/*
    1. 프로필 정보 관리
    2. 사용할 스킬 관리
    3. 사용할 아이템 관리
    4. 미니맵 관리
    5. 퀘스트 진행사항 관리
*/

public class UI_PlayScene : UI_Scene
{
    enum Gameobjects
    {
        SkillBar,
        ItemBar,
        ultSkillSlot,
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
    }

    enum Sliders
    {
        HpBar,
        MpBar,
        ExpBar,
    }

    enum Buttons
    {
        SpawnButton,
        LevelUpButton,
    }

    public UI_InvenPopup _inventory;        // 인벤토리 Popup UI
    public UI_EqStatPopup _equipment;       // 장비/스탯 Popup UI
    public UI_SkillPopup _skill;            // 스킬 Popup UI
    public UI_SlotTipPopup _slotTip;        // 슬롯팁 Popup UI
    public UI_ShopPopup _shop;              // 상점 Popup UI

    public override bool Init()
	{
		if (base.Init() == false)
			return false;

        BindObject(typeof(Gameobjects));
        BindImage(typeof(Images));
        BindButton(typeof(Buttons));
        BindText(typeof(Texts));
        Bind<Slider>(typeof(Sliders));

        _inventory = Managers.UI.ShowPopupUI<UI_InvenPopup>();
        _equipment = Managers.UI.ShowPopupUI<UI_EqStatPopup>();
        _skill = Managers.UI.ShowPopupUI<UI_SkillPopup>();
        _slotTip = Managers.UI.ShowPopupUI<UI_SlotTipPopup>();
        _shop = Managers.UI.ShowPopupUI<UI_ShopPopup>();
        Managers.Resource.Instantiate($"UI/SubItem/UI_DragSlot");

        SetInfo();

		return true;
	}

    void Update()
    {
        RefreshStat();
    }

    int skillBarCount = 6;
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

        // --

        GetObject((int)Gameobjects.ultSkillSlot).SetActive(false);

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
    }

    public void SetRatio(Slider slider, float ratio)
    {
        if (float.IsNaN(ratio) == true)
            slider.value = 0;
        else
            slider.value = ratio;
    }
}
