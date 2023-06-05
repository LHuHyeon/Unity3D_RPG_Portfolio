using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

        Managers.Game.isInventory = true;
        Managers.UI.OnPopupUI(Managers.Game._playScene._inventory);
        Managers.Game._playScene._inventory.ResetPos();
    }

    void ExitShop()
    {
        Managers.Game._playScene._shop.ExitShop();
    }
}
