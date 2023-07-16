using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
[ Scene 이동 포탈 ]
1. 플레이어가 Collider에 닿으면 포탈이 열리고, 나가면 닫힘.
2. 포탈에 플레이어가 닿으면 Scnce 이동.
*/

public class Portal : MonoBehaviour
{
    [SerializeField]
    Define.Scene sceneType;

    [SerializeField]
    GameObject portalObject;

    bool isPortal = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPortal = false;
            portalObject.SetActive(true);
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (portalObject.activeSelf == true && isPortal == false)
        {
            float distance = (Managers.Game.GetPlayer().transform.position - portalObject.transform.position).magnitude;
            if (distance <= 3.2f)
            {
                isPortal = true;
                Managers.Game.StopPlayer();

                if (Managers.Scene.CurrentScene.SceneType == Define.Scene.Game)
                {
                    Managers.UI.ShowPopupUI<UI_ConfirmPopup>().SetInfo(()=>
                    {
                        Managers.Game.SaveGame();

                        // 씬 이동 전 위치 저장
                        Managers.Game.CurrentPos += Vector3.forward * (-3f);

                        Managers.Scene.LoadScene(sceneType);
                    }, Define.DungeonMessage);
                }
                else
                    Managers.Scene.LoadScene(sceneType);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            portalObject.SetActive(false);
        }
    }
}
