using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorController : MonoBehaviour
{
    // RaycastHit hit;
    // Texture2D _attackIcon;
    // Texture2D _handIcon;

    // int _mask = (1 << (int)Define.Layer.Ground) | (1 << (int)Define.Layer.Monster);

    // // 마우스 커서 상태
    // public enum CursorType
    // {
    //     None,
    //     Attack,
    //     Hand,
    // }
    // CursorType _cursorType = CursorType.None;

    // void Start()
    // {
    //     // 커서 텍스쳐 가져오기
    //     _attackIcon = Managers.Resource.Load<Texture2D>("Textures/Cursor/Attack");
    //     _handIcon = Managers.Resource.Load<Texture2D>("Textures/Cursor/Hand");
    // }

    // void Update()
    // {
    //     // 마우스 클릭 중일 땐 커서 바뀌지 않게 하기!
    //     if (Input.GetMouseButton(0))
    //         return;
        
    //     Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

    //     if (Physics.Raycast(ray, out hit, 100f, _mask)){
    //         if (hit.collider.gameObject.layer == (int)Define.Layer.Monster){
    //             if (_cursorType != CursorType.Attack){
    //                 Cursor.SetCursor(_attackIcon, new Vector2(_attackIcon.width / 3.9f, 0), CursorMode.Auto);
    //                 _cursorType = CursorType.Attack;
    //             }
    //         }
    //         else{
    //             if (_cursorType != CursorType.Hand){
    //                 Cursor.SetCursor(_handIcon, new Vector2(_handIcon.width / 3.1f, 0), CursorMode.Auto);
    //                 _cursorType = CursorType.Hand;
    //             }
    //         }
    //     }
    // }
}
