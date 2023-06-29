using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
1. 플레이어가 Collider에 닿으면 포탈이 열리고, 나가면 닫힘.
2. 포탈에 플레이어가 닿으면 Scnce 이동.
*/

public class Portal : MonoBehaviour
{
    [SerializeField]
    Define.Scene sceneType;

    [SerializeField]
    GameObject portalObject;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            portalObject.SetActive(true);
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (portalObject.activeSelf == true)
        {
            float distance = (Managers.Game.GetPlayer().transform.position - portalObject.transform.position).magnitude;
            if (distance <= 3.2f)
            {
                Managers.Game.StopPlayer();
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
