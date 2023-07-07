using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_MenuPopup : UI_Popup
{
    enum Buttons
    {
        ContinueButton,
        SaveButton,
        AppExitButton,
    }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindButton(typeof(Buttons));

        popupType = Define.Popup.Menu;

        GetButton((int)Buttons.ContinueButton).onClick.AddListener(OnClickContinueButton);
        GetButton((int)Buttons.SaveButton).onClick.AddListener(OnClickSaveButton);
        GetButton((int)Buttons.AppExitButton).onClick.AddListener(OnClickAppExitButton);

        Managers.Input.KeyAction -= OnMenuPopup;
        Managers.Input.KeyAction += OnMenuPopup;

        Managers.UI.ClosePopupUI(this);

        return true;
    }

    void OnMenuPopup()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Managers.Game.isPopups[Define.Popup.Menu] = !Managers.Game.isPopups[Define.Popup.Menu];

            if (Managers.Game.isPopups[Define.Popup.Menu])
                OnMenu();
            else
                Exit();
        }
    }

    void OnMenu()
    {
        // 띄어진 Popup이 없다면
        if (Managers.UI.ClosePopupUI() == false)
        {
            Managers.UI.OnPopupUI(this);
            Time.timeScale = 0;
        }
        else
            Managers.Game.isPopups[Define.Popup.Menu] = false;
    }
    
    void OnClickContinueButton()
    {
        Exit();
    }

    void OnClickSaveButton()
    {
        Managers.Game.SaveGame();
        Exit();
    }

    void OnClickAppExitButton()
    {
        Managers.Game.SaveGame();
        Exit();
    }

    void Exit()
    {
        Time.timeScale = 1;
        Managers.Game.isPopups[Define.Popup.Menu] = false;
        Managers.UI.ClosePopupUI(this);
    }
}
