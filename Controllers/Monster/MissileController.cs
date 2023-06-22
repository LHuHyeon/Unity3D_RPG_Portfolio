using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

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

    void Update()
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
