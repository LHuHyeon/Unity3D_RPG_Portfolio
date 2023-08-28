using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * File :   EffectParticle.cs
 * Desc :   ParticleSystem의 물리적 접촉이 필요할 때 사용
 *
 & Functions
 &  [Public]
 &  : SetInfo()  - 설정
 &
 &  [Private]
 &  : ParticleCollider()    - 파티클 접촉 시 호출
 &  : OnParticleCollision() - 파티클 접촉 확인
 *
 */

public class EffectParticle : Effect
{
    Action _onParticleCollider;     // 파티클 접촉 시 실행시킬 기능 저장

    // 설정
    public void SetInfo(Action onParticleCollider)
    {
        _onParticleCollider = onParticleCollider;
    }

    // 파티클 접촉 시 호출
    private void ParticleCollider()
    {
        if (_onParticleCollider.IsNull() == false)
        {
            _onParticleCollider.Invoke();
            _onParticleCollider = null;
        }
    }

    // 파티클 물리적 접촉 확인
    private void OnParticleCollision(GameObject other)
    {
        // 플레이어가 접촉하면 True
        if (other.CompareTag("Player"))
            ParticleCollider();
    }
}
