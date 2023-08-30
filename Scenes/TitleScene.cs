using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * File :   TitleScene.cs
 * Desc :   TitleScene이 Load되면 호출된다.
 */

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
