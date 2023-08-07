using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
1. 플레이어의 스킬 이펙트에 붙여 Id로 사용한다.
*/

public class EffectData : MonoBehaviour
{
    public int id=0;
    public float disableDelayTime=0;    // effect 전용 비활성화 딜레이

    bool isEffect = false;

    void OnEnable()
    {
        GetComponent<ParticleSystem>().Play();
    }

    public void EffectDisableDelay()
    {
        if (disableDelayTime == 0)
        {
            gameObject.SetActive(false);
            return;
        }

        if (isEffect == false)
        {
            StopCoroutine(EffectDisableDelayTime());
            StartCoroutine(EffectDisableDelayTime());
        }
    }

    // 플레이어가 움직이더라도 스킬 이펙트가 활성화되야 한다면 사용
    IEnumerator EffectDisableDelayTime()
    {
        isEffect = true;

        Transform effectParent = transform.parent;   // 이펙트 부모
        Vector3 effectPos = transform.localPosition; // 이펙트 위치

        // 부모 빠져나오기
        transform.SetParent(null);
    
        // 이펙트 비활성화 기다리기
        yield return new WaitForSeconds(disableDelayTime);

        // 원위치 이동 후 비활성화
        transform.SetParent(effectParent);
        transform.localPosition = effectPos;
        transform.localRotation = Quaternion.identity;

        isEffect = false;

        gameObject.SetActive(false);
    }
}
