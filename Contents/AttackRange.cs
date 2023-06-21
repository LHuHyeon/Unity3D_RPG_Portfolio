using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackRange : MonoBehaviour
{
    GameObject player;
    MonsterStat _stat;

    bool _isDown = false;

    public void SetInfo(MonsterStat monsterStat, bool isDown)
    {
        _stat = monsterStat;
        _isDown = isDown;
    }

    // 비활성화 될때 플레이어와 접촉 중이면 데미지 주기
    void OnDisable()
    {
        if (player == null)
            return;

        if (_isDown == true)
        {
            Managers.Game.GetPlayer().GetComponent<PlayerController>().OnHitDown(_stat, (int)(_stat.Attack / 2));
        }
        else
            Managers.Game.OnAttacked(_stat, (int)(_stat.Attack / 2));
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            player = other.gameObject;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            player = null;
    }
}
