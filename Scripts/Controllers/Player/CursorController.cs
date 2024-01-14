using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * File :   CursorController.cs
 * Desc :   마우스 커서 icon을 상황마다 바꿔주는 기능
 *
 & Functions
 &  [Private]
 &  : CursorUpdate() - 상황마다 마우스 커서 Update
 *
 */

public class CursorController : MonoBehaviour
{
    // 마우스 커서 상태
    public enum CursorType
    {
        None,
        Attack,
        Hand,
        Loot,
    }

    private CursorType  _cursorType = CursorType.None;

    private RaycastHit  hit;
    private Texture2D   _attackIcon;  // 공격 icon
    private Texture2D   _handIcon;    // 기본 icon
    private Texture2D   _lootIcon;    // npc icon

    private int         _mask = (1 << (int)Define.Layer.Ground) | (1 << (int)Define.Layer.Monster) | (1 << (int)Define.Layer.Npc);

    void Start()
    {
        // 커서 텍스쳐 가져오기
        _attackIcon = Managers.Resource.Load<Texture2D>("Textures/Cursor/Attack");
        _handIcon = Managers.Resource.Load<Texture2D>("Textures/Cursor/Hand");
        _lootIcon = Managers.Resource.Load<Texture2D>("Textures/Cursor/Loot");

        // Hand icon 커서에 적용 
        Cursor.SetCursor(_handIcon, new Vector2(_handIcon.width / 3.1f, 0), CursorMode.Auto);
        _cursorType = CursorType.Hand;
    }

    void Update()
    {
        CursorUpdate();
    }

    private void CursorUpdate()
    {
        // 꾹 누르면 아이콘 유지
        if (Input.GetMouseButton(0))
            return;

        // 마우스 포인트 가져오기
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, 100f, _mask))
        {
            // NPC
            if (hit.collider.gameObject.layer == (int)Define.Layer.Npc)
            {
                if (_cursorType != CursorType.Loot)
                {
                    Cursor.SetCursor(_lootIcon, new Vector2(_lootIcon.width / 4.5f, _lootIcon.height / 2), CursorMode.Auto);
                    _cursorType = CursorType.Loot;
                }
                return;
            }
            // Monster
            else if (hit.collider.gameObject.layer == (int)Define.Layer.Monster)
            {
                if (_cursorType != CursorType.Attack)
                {
                    Cursor.SetCursor(_attackIcon, new Vector2(_attackIcon.width / 3.9f, 0), CursorMode.Auto);
                    _cursorType = CursorType.Attack;
                }
                return;
            }
            // Default
            else if (_cursorType != CursorType.Hand)
            {
                Cursor.SetCursor(_handIcon, new Vector2(_handIcon.width / 3.1f, 0), CursorMode.Auto);
                _cursorType = CursorType.Hand;
            }
        }
    }
}