using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * File :   UI_Scene.cs
 * Desc :   모든 Scene의 부모
 *
 & Functions
 &  [Public]
 &  : Init()    - 초기 설정
 *
 */

public class UI_Scene : UI_Base
{
    public override bool Init()
	{
		if (base.Init() == false)
			return false;

		Managers.UI.SetCanvas(gameObject, false);
		return true;
	}
}
