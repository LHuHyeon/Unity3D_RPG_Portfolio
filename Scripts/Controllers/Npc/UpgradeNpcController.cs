using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * File :   UpgradeNpcController.cs
 * Desc :   장비 강화 NPC 기능 구현
 *
 & Functions
 &  [Protected]
 &  : OpenPopup()   - Popup 활성화   (OpenUpgrade() 호출)
 &  : ExitPopup()   - Popup 비활성화 (ExitUpgrade() 호출)
 &
 &  [Private]
 &  : OpenUpgrade() - 업그레이드 Popup 열기
 &  : ExitUpgrade() - 업그레이드 Popup 나가기
 *
 */

public class UpgradeNpcController : NpcController
{
    protected override void OpenPopup() { OpenUpgrade(); }
    protected override void ExitPopup() { ExitUpgrade(); }

    private void OpenUpgrade()
    {
        // 업그레이드 Popup 활성화
        Managers.UI.OnPopupUI(Managers.Game._playScene._upgrade);

        // 인벤토리 Popup 활성화
        Managers.UI.OnPopupUI(Managers.Game._playScene._inventory);
        Managers.Game._playScene._inventory.ResetPos();
    }

    private void ExitUpgrade()
    {
        Managers.Game._playScene._upgrade.ExitUpgrade();
    }
}
