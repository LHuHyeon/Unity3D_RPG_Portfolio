using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimEvent : MonoBehaviour
{
    [SerializeField]
    private CapsuleCollider capsuleCollider;

    // 공격 사이즈 클래스
    public class AttackSize
    {
        public float x;
        public float y;
        public float z;
        public float redius;
        public float height;
    }

    // 스킬Id 101 사이즈
    private AttackSize skill101 = new AttackSize()
    {
        x = 0, y = 0, z = -0.35f, redius = 2.35f, height = 4.5f
    };

    // 기본 검 공격
    public void OnBasicAttack()
    {
        capsuleCollider.gameObject.SetActive(true);
    }

    public void OnComboAttack()
    {
        capsuleCollider.gameObject.SetActive(true);
        SetSize(skill101);
    }

    private void SetSize(AttackSize size)
    {
        capsuleCollider.center.Set(size.x, size.y, size.z);
        capsuleCollider.radius = size.redius;
        capsuleCollider.height = size.height;
    }
}
