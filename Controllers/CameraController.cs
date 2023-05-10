using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private Define.CameraMode _mode = Define.CameraMode.QuarterView;
    [SerializeField]
    private Vector3 _delta;
    [SerializeField]
    private GameObject _player = null;

    public void SetPlayer(GameObject go) { _player = go; }

    RaycastHit hit;
    
    // 카메라 위치 이동을 마지막 업데이트에 실행함으로 써 떨림현상 완화
    void LateUpdate()
    {
        if (_mode == Define.CameraMode.QuarterView){
            if (_player.isValid() == false)
                return;

            // 플레이어가 오브젝트에 가려져있다면 가깝게 이동
            if (Physics.Raycast(_player.transform.position, _delta, out hit, _delta.magnitude, LayerMask.GetMask("Block"))){
                float dist = (hit.point - _player.transform.position).magnitude * 0.8f;
                transform.position = (_player.transform.position + Vector3.up) + _delta.normalized * dist;
            }
            else{
                transform.position = _player.transform.position + _delta;
                transform.LookAt(_player.transform);
            }
        }
    }

    // 카메라 위치 메소드
    public void SetQuaterView(Vector3 delta)
    {
        _mode = Define.CameraMode.QuarterView;
        _delta = delta;
    }
}
