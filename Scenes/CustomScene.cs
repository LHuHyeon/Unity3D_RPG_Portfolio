using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 캐릭터 커스텀 씬
public class CustomScene : BaseScene
{
    [SerializeField]
    Transform characterPos;

    protected override void Init()
    {
        base.Init();

        SceneType = Define.Scene.PlayerCustom;

        GameObject charCustom = Managers.Resource.Instantiate("CharacterCustom", characterPos);
        Managers.UI.ShowSceneUI<UI_CustomScene>().custom = charCustom.GetComponent<CharacterCustom>();
    }

    public override void Clear() {}
}
