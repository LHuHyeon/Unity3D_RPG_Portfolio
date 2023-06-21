using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectAttackRange : MonoBehaviour
{
    float _effectTime;

    GameObject player;
    GameObject _effect;

    MonsterStat _stat;

    public void SetInfo(MonsterStat monsterStat, string effectPath, float effectTime)
    {
        _stat = monsterStat;
        _effect = Managers.Resource.Instantiate(effectPath, this.transform);
        _effectTime = effectTime;
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

    IEnumerator DelayDisable()
    {
        yield return new WaitForSeconds(_effectTime);

        if (player != null)
            Managers.Game.OnAttacked(_stat, (int)(_stat.Attack / 2));

        Managers.Resource.Destroy(_effect);
        Managers.Resource.Destroy(this.gameObject);
    }
}
