using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackCollider : MonoBehaviour
{
    public int damage;

    [SerializeField]
    private BoxCollider boxCollider;

    public void IsCollider(bool isActive) { boxCollider.enabled = isActive; }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") == true)
            Managers.Game.OnAttacked(damage);
    }
}
