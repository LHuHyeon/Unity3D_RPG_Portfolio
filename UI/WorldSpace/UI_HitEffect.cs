using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/*
 * File :   UI_HitEffect.cs
 * Desc :   Monster 피격 시 생성되는 데미지 Effect UI
 *
 & Functions
 &  OnEnable()          - 활성화 시 DelayDisalbe() 코루틴 시작
 &  FixedUpdate()       - 높이 증가시키기
 &
 &  [Private]
 &  : DelayDisalbe()    - 딜레이 비활성화 (Coroutine)
 *
 */

public class UI_HitEffect : UI_Base
{
    public TextMeshProUGUI      hitText;

    public float                upSpeed = 0.1f;

    void OnEnable()
    {
        StartCoroutine(DelayDisalbe());
    }
    
    void FixedUpdate()
    {
        hitText.transform.rotation = Camera.main.transform.rotation;

        transform.position += Vector3.up * upSpeed * Time.deltaTime;
    }

    private IEnumerator DelayDisalbe()
    {
        yield return new WaitForSeconds(2f);

        Managers.Resource.Destroy(gameObject);
    }
}
