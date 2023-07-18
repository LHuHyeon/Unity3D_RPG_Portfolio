using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
[ 맵 카메라 컨트롤러 스크립트 ]
1. 미니맵 전용 카메라다.
*/

public class MapCameraController : MonoBehaviour
{
    [SerializeField]
    private float height;
    
    void FixedUpdate()
    {
        if (Managers.Game.GetPlayer().isValid() == false)
            return;

        // 플레이어 따라다니기
        transform.position = Managers.Game.GetPlayer().transform.position + (Vector3.up * height);
    }
}
