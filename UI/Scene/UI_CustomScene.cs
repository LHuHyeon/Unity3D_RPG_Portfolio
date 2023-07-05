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

        Managers.UI.ShowPopupUI<UI_LoadPopup>().SetInfo(Define.Scene.Game, 7);
    }

    void OnClickExitButton()
    {
        Managers.Scene.LoadScene(Define.Scene.Title);
    }
}
