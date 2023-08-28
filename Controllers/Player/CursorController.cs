using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
커서의 icon을 상황마다 바꿔주는 스크립트이다.
*/

public class CursorController : MonoBehaviour
{
    RaycastHit hit;
    Texture2D _attackIcon;  // 공격 icon
    Texture2D _handIcon;    // 기본 icon
    Texture2D _lootIcon;    // npc icon

    int _mask = (1 << (int)Define.Layer.Ground) | (1 << (int)Define.Layer.Monster) | (1 << (int)Define.Layer.Npc);

    // 마우스 커서 상태
    public enum CursorType
    {
        None,
        Attack,
        Hand,
        Loot,
    }
    CursorType _cursorType = CursorType.None;

    void Start()
    {
        // 커서 텍스쳐 가져오기
        _attackIcon = Managers.Resource.Load<Texture2D>("Textures/Cursor/Attack");
        _handIcon = Managers.Resource.Load<Texture2D>("Textures/Cursor/Hand");
        _lootIcon = Managers.Resource.Load<Texture2D>("Textures/Cursor/Loot");

        Cursor.SetCursor(_handIcon, new Vector2(_handIcon.width / 3.1f, 0), CursorMode.Auto);
        _cursorType = CursorType.Hand;
    }

    void Update()
    {
        // 마우스 클릭 중일 땐 커서 바뀌지 않게 하기!
        if (Input.GetMouseButton(0))
            return;
        
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, 100f, _mask))
        {
            if (hit.collider.gameObject.layer == (int)Define.Layer.Monster)
            {
                if (_cursorType != CursorType.Attack)
                {
                    Cursor.SetCursor(_attackIcon, new Vector2(_attackIcon.width / 3.9f, 0), CursorMode.Auto);
                    _cursorType = CursorType.Attack;
                }
            }
            else if (hit.collider.gameObject.layer == (int)Define.Layer.Npc)
            {
                if (_cursorType != CursorType.Loot)
                {
                    Cursor.SetCursor(_lootIcon, new Vector2(_lootIcon.width / 4.5f, _lootIcon.height / 2), CursorMode.Auto);
                    _cursorType = CursorType.Loot;
                }
            }
            else
            {
                if (_cursorType != CursorType.Hand)
                {
                    Cursor.SetCursor(_handIcon, new Vector2(_handIcon.width / 3.1f, 0), CursorMode.Auto);
                    _cursorType = CursorType.Hand;
                }
            }
        }
    }
}