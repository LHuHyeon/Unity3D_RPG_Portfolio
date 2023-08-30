using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * File :   MapCameraController.cs
 * Desc :   미니맵 전용 카메라
 *
 & Functions
 &  : FixedUpdate() - 하늘에서 플레이어 추격
 *
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
