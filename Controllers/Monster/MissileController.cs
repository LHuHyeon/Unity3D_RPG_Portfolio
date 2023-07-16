using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/*
[ 미사일 컨트롤러 스크립트 ]
1. 일정 시간동안 타겟을 따라다녀 부딪칠 시 데미지를 입힌다.
2. 현재 보스 유도 미사일 공격으로 사용 중이다.
*/

public class MissileController : MonoBehaviour
{
    MonsterStat _stat;

    float _disableTime = 0;

    NavMeshAgent nav;

    void Start()
    {
        nav = GetComponent<NavMeshAgent>();
    }

    public void SetInfo(MonsterStat stat, float disableTime)
    {
        _stat = stat;
        _disableTime = disableTime;

        StartCoroutine(this.DelayDisable());
    }

    void FixedUpdate()
    {
        nav.SetDestination(Managers.Game.GetPlayer().transform.position);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Managers.Game.OnAttacked(_stat);
            Managers.Resource.Destroy(this.gameObject);
        }
    }

    IEnumerator DelayDisable()
    {
        yield return new WaitForSeconds(_disableTime);

        Managers.Resource.Destroy(this.gameObject);
    }
}
