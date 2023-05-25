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

    public UI_InvenPopup _inventory;
    public UI_EqStatPopup _equipment;
    public UI_SkillPopup _skill;
    public UI_SlotTipPopup _slotTip;

    public override bool Init()
	{
		if (base.Init() == false)
			return false;

        BindObject(typeof(Gameobjects));
        BindImage(typeof(Images));
        BindText(typeof(Texts));
        Bind<Slider>(typeof(Sliders));

        _inventory = Managers.UI.ShowPopupUI<UI_InvenPopup>();
        _equipment = Managers.UI.ShowPopupUI<UI_EqStatPopup>();
        _skill = Managers.UI.ShowPopupUI<UI_SkillPopup>();
        _slotTip = Managers.UI.ShowPopupUI<UI_SlotTipPopup>();
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

        // foreach(Transform child in GetObject((int)Gameobjects.SkillBar).transform)
        //     Managers.Resource.Destroy(child.gameObject);

        // for(int i=0; i<skillBarCount; i++)
        //     Managers.UI.MakeSubItem<UI_InvenItem>(parent: GetObject((int)Gameobjects.SkillBar).transform);

        RefreshUI();
    }

    public void RefreshUI()
    {
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
