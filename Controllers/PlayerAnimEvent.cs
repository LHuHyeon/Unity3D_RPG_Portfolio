using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimEvent : MonoBehaviour
{
    [SerializeField]
    private CapsuleCollider capsuleCollider;

    private int nextSkillIndex = 0;

    // 공격 사이즈 클래스
    public class AttackSize
    {
        public float x;
        public float y;
        public float z;
        public float redius;
        public float height;
        public int direction;   // x: 0, y: 1, z: 2
    }

    // Id 101 공격 범위 (트리플 슬래쉬)
    private AttackSize skill101 = new AttackSize()
    {
        x = 0, y = 0, z = -0.35f, redius = 2.35f, height = 4.5f, direction = 1,
    };

    // Id 102 공격 범위 (라이징 슬래쉬)
    private AttackSize[] skill102 = new AttackSize[]
    {
        new AttackSize()
        {
            x = 0.3f, y = 0, z = 1.23f, redius = 0.5f, height = 3.5f, direction = 2,
        },
        new AttackSize()
        {
            x = 0.43f, y = 0.56f, z = 1f, redius = 0.93f, height = 3.6f, direction = 1,
        },
        new AttackSize()
        {
            x = 0, y = 0, z = 0f, redius = 2.35f, height = 4.5f, direction = 1,
        },
    };

    // 기본 검 공격
    public void OnBasicAttack()
    {
        capsuleCollider.gameObject.SetActive(true);
    }

    // skill 101 : 트리플 슬래쉬
    public void OnTripleSlash()
    {
        capsuleCollider.gameObject.SetActive(true);
        SetSize(skill101);
    }

    // skill 102 : 라이징 슬래쉬
    public void OnRisingSlash()
    {
        capsuleCollider.gameObject.SetActive(true);
        SetSize(skill102[nextSkillIndex]);
        
        ++nextSkillIndex;
        if (nextSkillIndex == skill102.Length)
            nextSkillIndex = 0;
    }

    private void SetSize(AttackSize size)
    {
        capsuleCollider.center.Set(size.x, size.y, size.z);
        capsuleCollider.radius = size.redius;
        capsuleCollider.height = size.height;
        capsuleCollider.direction = size.direction;
    }
}