using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
[ 상점 NPC 컨트롤러 스크립트 ]
1. 플레이어와 상호작용하면 상점 UI를 활성화 한다. (UI_ShopPopup)
*/

public class ShopNpcController : NpcController
{
    public Define.ShopType shopType = Define.ShopType.Unknown;

    [SerializeField] private int shopBuyId;

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
            OnShop();
        }
        else
            ExitShop();
    }

    void OnShop()
    {
        Managers.UI.OnPopupUI(Managers.Game._playScene._shop);
        Managers.Game._playScene._shop.RefreshUI(this, shopBuyId);

        Managers.UI.OnPopupUI(Managers.Game._playScene._inventory);
        Managers.Game._playScene._inventory.ResetPos();
    }

    void ExitShop()
    {
        Managers.Game._playScene._shop.ExitShop();
    }
}
