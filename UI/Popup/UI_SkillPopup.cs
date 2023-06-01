using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_SkillPopup : UI_Popup
{
    enum Gameobjects
    {

    }

    enum Texts
    {

    }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;
    
        BindObject(typeof(Gameobjects));
        BindText(typeof(Texts));

        Managers.Input.KeyAction -= OnSkillPopup;
        Managers.Input.KeyAction += OnSkillPopup;

        SetInfo();

        gameObject.SetActive(false);

        return true;
    }

    void OnSkillPopup()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            Managers.Game.isSkillUI = !Managers.Game.isSkillUI;

            if (Managers.Game.isSkillUI)
                Managers.UI.OnPopupUI(this);
            else
                Managers.Resource.Destroy(gameObject);
        }
    }

    public void SetInfo()
    {
        
    }
}
