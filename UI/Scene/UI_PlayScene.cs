using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_PlayScene : UI_Scene
{
    /*
    1. 프로필 정보 관리
    2. 사용할 스킬 관리
    3. 사용할 아이템 관리
    4. 미니맵 관리
    5. 퀘스트 진행사항 관리
    */
    public override bool Init()
	{
		if (base.Init() == false)
			return false;


		return true;
	}

    public void SetInfo()
    {

    }

    void RefreshUI()
    {

    }
}
