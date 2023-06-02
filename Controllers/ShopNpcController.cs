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
        Debug.Log(npcName + " : OnInteract");
        if (Managers.Game.IsInteract)
        {
            Managers.Game.StopPlayer();
            Managers.UI.OnPopupUI(Managers.Game._playScene._shop);
            Managers.Game._playScene._shop.RefreshUI(this);
        }
        else
            Managers.UI.ClosePopupUI(Managers.Game._playScene._shop);
    }
}
