using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_CustomScene : UI_Scene
{
    enum GameObjects
    {
        Grid,
    }

    enum Buttons
    {
        CheckButton,
        ExitButton,
    }

    public CharacterCustom custom;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindObject(typeof(GameObjects));
        BindButton(typeof(Buttons));

        if (custom == null)
            custom = GameObject.FindObjectOfType<CharacterCustom>();

        foreach(Transform child in GetObject((int)GameObjects.Grid).transform)
            child.GetComponent<UI_CustomButton>().SetInfo(custom);

        GetButton((int)Buttons.CheckButton).onClick.AddListener(OnClickCheckButton);
        GetButton((int)Buttons.ExitButton).onClick.AddListener(OnClickExitButton);

        return true;
    }

    void OnClickCheckButton()
    {
        custom.SaveCustom();

        Managers.UI.ShowPopupUI<UI_InputPopup>().SetInfo((string inputText)=>
        {
            Managers.Game.Name = inputText;
            LoadPopup();
        }, "이름을 입력해 주세요", "이름 입력란", Define.NameRegex);
    }

    void LoadPopup()
    {
        if(Application.internetReachability == NetworkReachability.NotReachable)
        {
            // 인터넷 연결이 안되었을 때 행동
            Managers.UI.ShowPopupUI<UI_GuidePopup>().SetInfo("네트워크 연결이 필요합니다.", Color.red);
        }
        else if(Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork)
        {
            // 데이터로 연결이 되었을 때 행동
            Managers.UI.ShowPopupUI<UI_LoadPopup>().SetInfo(Define.Scene.Game, 6);
        }
        else
        {
            // 와이파이로 연결이 되었을 때 행동
            Managers.UI.ShowPopupUI<UI_LoadPopup>().SetInfo(Define.Scene.Game, 7);
        }
    }

    void OnClickExitButton()
    {
        Managers.Scene.LoadScene(Define.Scene.Title);
    }
}
