using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_DiePopup : UI_Popup
{
    enum Buttons
    {
        ResurrectionButton,
        ReSpawnButton,
    }

    enum Images { Background }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindButton(typeof(Buttons));
        BindImage(typeof(Images));

        // 즉시 부활 버튼
        GetButton((int)Buttons.ResurrectionButton).onClick.AddListener(()=>
        {
            // 제자리 부활 + 체력/마나 50% 회복 + 100골드 차감
            if (Managers.Game.Gold < 100)
            {
                Managers.UI.ShowPopupUI<UI_GuidePopup>().SetInfo("골드가 부족합니다!", Color.yellow);
                return;
            }

            Managers.Game.Gold -= 100;

            Managers.Game.OnResurrection(0.5f);

            Managers.UI.ClosePopupUI(this);
        });

        // 마을 부활 버튼 
        GetButton((int)Buttons.ReSpawnButton).onClick.AddListener(()=>
        {
            // 마을 부활 + 체력/마나 20% 회복
            Managers.Game.OnResurrection(0.2f);

            if (Managers.Scene.CurrentScene.SceneType != Define.Scene.Game)
                Managers.Scene.LoadScene(Define.Scene.Game);
                
            Managers.Game.GetPlayer().transform.position = Managers.Game.defualtSpawn;
            
            Managers.UI.ClosePopupUI(this);
        });

        return true;
    }
}
