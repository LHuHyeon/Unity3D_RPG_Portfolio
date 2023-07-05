using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleScene : BaseScene
{
    protected override void Init()
    {
        base.Init();

        SceneType = Define.Scene.Title;

        Managers.UI.ShowSceneUI<UI_TitleScene>();
    }

    public override void Clear() {}
}
