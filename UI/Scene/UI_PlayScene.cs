using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_PlayScene : UI_Scene
{
    /*
    1. 프로필 정보 관리
    2. 사용할 스킬 관리
    3. 사용할 아이템 관리
    4. 미니맵 관리
    5. 퀘스트 진행사항 관리
    */

    enum Gameobejcts
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

    public override bool Init()
	{
		if (base.Init() == false)
			return false;

        BindObject(typeof(Gameobejcts));
        BindImage(typeof(Images));
        BindText(typeof(Texts));
        Bind<Slider>(typeof(Sliders));

        SetInfo();

		return true;
	}

    void Update()
    {
        RefreshStat();
    }

    public void SetInfo()
    {
        GetText((int)Texts.NameBarText).text = Managers.Game.Name;

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
