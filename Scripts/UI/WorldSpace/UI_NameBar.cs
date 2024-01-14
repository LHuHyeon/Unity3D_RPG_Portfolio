using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/*
 * File :   UI_NameBar.cs
 * Desc :   Item 위에 생성되는 Name바 UI
 *
 & Functions
 &  Init()          - 초기 설정 
 &  FixedUpdate()   - 객체 상단 위치 고정 및 카메라 바라보도록 회전
 *
 */

public class UI_NameBar : UI_Base
{
    enum Gameobjects
    {
        Background,
    }

    enum Texts
    {
        NameText,
    }

    public string       nameText;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindObject(typeof(Gameobjects));
        BindText(typeof(Texts));

        GetText((int)Texts.NameText).text = nameText;

        return true;
    }

    void FixedUpdate()
    {
        Transform parent = transform.parent;
        float valueY = (parent.GetComponent<Collider>().bounds.size.y * 1.3f);

        transform.position = parent.position + Vector3.up * valueY;
        GetObject((int)Gameobjects.Background).transform.rotation = Camera.main.transform.rotation;
    }
}
