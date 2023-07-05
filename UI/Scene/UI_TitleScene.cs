using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_TitleScene : UI_Scene
{
    enum Buttons
    {
        StartButton,
        LoadButton,
        ExitButton,
    }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindButton(typeof(Buttons));

        GetButton((int)Buttons.StartButton).onClick.AddListener(OnClickStartButton);
        GetButton((int)Buttons.LoadButton).onClick.AddListener(OnClickLoadButton);
        GetButton((int)Buttons.ExitButton).onClick.AddListener(OnClickExitButton);

        return true;
    }
    
    void OnClickStartButton()
    {
        Managers.Scene.LoadScene(Define.Scene.PlayerCustom);
    }

    void OnClickLoadButton()
    {
        // TODO : 세이브 로드
    }

    void OnClickExitButton()
    {
        Application.Quit();
    }
}
