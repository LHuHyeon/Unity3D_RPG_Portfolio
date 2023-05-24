using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// 몬스터 피격시 나오는 데미지 이펙트
public class UI_HitEffect : UI_Base
{
    public TextMeshProUGUI hitText;

    void OnEnable()
    {
        StartCoroutine(DelayDisalbe());
    }

    public float upSpeed = 0.1f;
    void FixedUpdate()
    {
        hitText.transform.rotation = Camera.main.transform.rotation;

        transform.position += Vector3.up * upSpeed * Time.deltaTime;
    }

    IEnumerator DelayDisalbe()
    {
        yield return new WaitForSeconds(2f);

        Managers.Resource.Destroy(gameObject);
    }
}
