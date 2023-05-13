using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimEvent : MonoBehaviour
{
    [SerializeField]
    private BoxCollider boxCollider;

    // 기본 검 공격
    public void OnBasicAttack()
    {
        boxCollider.gameObject.SetActive(true);
    }
}
