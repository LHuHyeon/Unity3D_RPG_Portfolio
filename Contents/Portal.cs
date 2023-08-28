using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * File :   Portal.cs
 * Desc :   포탈 생성 및 Scene 이동
 *
 & Functions
 &  : OnTriggerEnter()  - 플레이어 Enter 체크 후 포탈 활성화
 &  : OnTriggerStay()   - 플레이어와 근접하면 Scene Load
 &  : OnTriggerExit()   - 플레이어 Exit 체크 후 포탈 비활성화
 *
 */

public class Portal : MonoBehaviour
{
    private float           scanRange = 3.2f;   // 플레이어 스캔 거리
    private bool            isPortal  = false;  // 포탈 접촉 여부

    [SerializeField]
    private Define.Scene    sceneType;          // Load할 Scene 타입

    [SerializeField]
    private GameObject      portalObject;       // 포탈 객체

    void OnTriggerEnter(Collider other)
    {
        // 플레이어 체크
        if (other.CompareTag("Player"))
        {
            isPortal = false;
            portalObject.SetActive(true);
        }
    }

    void OnTriggerStay(Collider other)
    {
        // 포탈 활성화 체크
        if (portalObject.activeSelf == true && isPortal == false)
        {
            // 플레이어와 포탈이 근접한지 체크
            float distance = (Managers.Game.GetPlayer().transform.position - portalObject.transform.position).magnitude;
            if (distance <= scanRange)
            {
                isPortal = true;

                // 플레이어 정지
                Managers.Game.StopPlayer();

                // 현재 Scene이 Game Scene라면
                if (Managers.Scene.CurrentScene.SceneType == Define.Scene.Game)
                {
                    // 확인 Popup 활성화
                    UI_ConfirmPopup confirmPopup = Managers.UI.ShowPopupUI<UI_ConfirmPopup>();
                    if (confirmPopup.IsNull() == true)
                        return;
                    
                    // 확인 Popup 설정
                    confirmPopup.SetInfo(()=>
                    {
                        // 게임 세이브
                        Managers.Game.SaveGame();

                        // 씬 이동 전 위치 저장
                        Managers.Game.CurrentPos += Vector3.forward * (-3f);

                        // 씬 로드
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
