using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * File :   ShopNpcController.cs
 * Desc :   상점 Npc 기능 구현
 *
 & Functions
 &  [Protected]
 &  : OpenPopup()   - Popup 활성화   (OpenShop() 호출)
 &  : ExitPopup()   - Popup 비활성화 (ExitShop() 호출)
 &
 &  [Private]
 &  : OpenShop()    - 상점 Popup 열기
 &  : ExitShop()    - 상점 Popup 나가기
 *
 */

public class ShopNpcController : NpcController
{
    public Define.ShopType  shopType = Define.ShopType.Unknown;

    [SerializeField]
    private int             shopBuyId;      // Shop Npc 구매 목록 Id

    protected override void OpenPopup() { OpenShop(); }
    protected override void ExitPopup() { ExitShop(); }

    private void OpenShop()
    {
        // 상점 Popup 활성화
        Managers.UI.OnPopupUI(Managers.Game._playScene._shop);
        Managers.Game._playScene._shop.RefreshUI(this, shopBuyId);

        // 인벤토리 Popup 활성화
        Managers.UI.OnPopupUI(Managers.Game._playScene._inventory);
        Managers.Game._playScene._inventory.ResetPos();
    }

    private void ExitShop()
    {
        Managers.Game._playScene._shop.ExitShop();
    }
}
