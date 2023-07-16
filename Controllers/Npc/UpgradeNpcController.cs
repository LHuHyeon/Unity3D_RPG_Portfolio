using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
[ 업그레이드 NPC 컨트롤러 스크립트 ]
1. 플레이어와 상호작용하면 업그레이드 UI를 활성화 한다. (UI_UpgradePopup)
*/

public class UpgradeNpcController : NpcController
{
    public override void Init()
    {
        base.Init();
    }

    public override void Interact()
    {
        if (Managers.Game.IsInteract)
        {
            Managers.UI.CloseAllPopupUI();
            Managers.Game.StopPlayer();
            OnUpgrade();
        }
        else
            ExitUpgrade();
    }

    void OnUpgrade()
    {
        Managers.UI.OnPopupUI(Managers.Game._playScene._upgrade);
        Managers.UI.OnPopupUI(Managers.Game._playScene._inventory);
        Managers.Game._playScene._inventory.ResetPos();
    }

    void ExitUpgrade()
    {
        Managers.Game._playScene._upgrade.ExitUpgrade();
    }
}
