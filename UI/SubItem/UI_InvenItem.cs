using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_InvenItem : UI_Base
{
    /*
    1. EventSystem 사용하여 마우스로 아이템 이동, 바꾸기, PlayScene 사용창으로 이동, 장비창으로 이동 
    2. (전포트폴리오 참고)
    */
    enum Images
    {
        ItemImage,
    }

    enum Texts
    {
        ItemCountText,
    }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindImage(typeof(Images));
        BindText(typeof(Texts));

        return true;
    }

    public void SetInfo()
    {

    }

    void RefreshUI()
    {

    }
}
