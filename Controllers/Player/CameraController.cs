using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * File :   CameraController.cs
 * Desc :   플레이어를 쿼터뷰 모드로 따라다니는 카메라 기능
 *
 & Functions
 &  [Public]
 &  : SetPlayer()           - 플레이어 Prefab 받기
 &
 &  [Private]
 &  : QuarterViewUpdate()   - 쿼터뷰 모드로 플레이어 따라가기
 *
 */

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private Define.CameraMode   _mode = Define.CameraMode.QuarterView;
    [SerializeField]
    private Vector3             _delta;
    [SerializeField]
    private GameObject          _player = null;

    private RaycastHit          hit;

    public void SetPlayer(GameObject go) { _player = go; }
    
    void LateUpdate()
    {
        QuarterViewUpdate();
    }

    // 카메라 위치 이동을 마지막 업데이트에 실행함으로 써 떨림현상 완화
    private void QuarterViewUpdate()
    {
        if (_mode == Define.CameraMode.QuarterView)
        {
            if (_player.isValid() == false)
                return;

            // 플레이어가 오브젝트에 가려져있다면 가깝게 이동
            if (Physics.Raycast(_player.transform.position, _delta, out hit, _delta.magnitude, 1 << 10)) // 10 : Block
            {
                float dist = (hit.point - _player.transform.position).magnitude * 0.8f;
                transform.position = (_player.transform.position + Vector3.up) + _delta.normalized * dist;
            }
            else
            {
                transform.position = _player.transform.position + _delta;
                transform.LookAt(_player.transform);
            }
        }
    }
}
