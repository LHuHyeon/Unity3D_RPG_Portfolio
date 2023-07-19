using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
[ Scene 스크립트 ]
1. 모든 Scene의 부모이다.
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
