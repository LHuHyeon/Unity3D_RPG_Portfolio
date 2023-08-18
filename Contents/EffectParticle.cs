using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectParticle : Effect
{
    Action _onParticleCollider;

    public void SetInfo(Action onParticleCollider)
    {
        _onParticleCollider = onParticleCollider;
    }

    protected virtual void ParticleCollider()
    {
        if (_onParticleCollider.IsNull() == false)
        {
            _onParticleCollider.Invoke();
            _onParticleCollider = null;
        }
    }

    private void OnParticleCollision(GameObject other)
    {
        if (other.CompareTag("Player"))
            ParticleCollider();
    }
}
