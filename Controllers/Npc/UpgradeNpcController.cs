using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

        Managers.Game.isPopups[Define.Popup.Inventory] = true;
        Managers.UI.OnPopupUI(Managers.Game._playScene._inventory);
        Managers.Game._playScene._inventory.ResetPos();
    }

    void ExitUpgrade()
    {
        Managers.Game._playScene._upgrade.ExitUpgrade();
    }
}
