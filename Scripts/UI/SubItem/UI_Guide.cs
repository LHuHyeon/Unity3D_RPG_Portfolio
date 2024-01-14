using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/*
 * File :   UI_Guide.cs
 * Desc :   안내문, 경고문 등 상황에 띄울 수 있는 가이드 UI
 *
 & Functions
 &  [Public]
 &  : SetInfo()             - 기능 설정 (안내 메시지 설정)
 &
 &  [Private]
 &  : MessageCoroutine()    - 메시지가 붕뜨며 사라지는 코루틴
 *
 */

public class UI_Guide : UI_Base
{
    [SerializeField]
    private TextMeshProUGUI     _messageText;
    private Color               _color;
    private Coroutine           co;

    public void SetInfo(string messageText, Color color)
    {
        // 초기화
        _messageText.text = messageText;
        _messageText.transform.localPosition = Vector3.zero;
        _color = color;
        _messageText.color = _color;

        if (co.IsNull() == false) StopCoroutine(co);
        co = StartCoroutine(MessageCoroutine());
    }

    private IEnumerator MessageCoroutine()
    {
        yield return new WaitForSeconds(1f);

        // 점점 사라지며 올라가기
        for(float i=1.0f; i>=0.0f; i-=0.01f)
        {
            _color.a = i;
            _messageText.color = _color;

            _messageText.transform.localPosition += Vector3.up * 0.7f;

            yield return null;
        }
        
        Managers.Resource.Destroy(gameObject);
    }
}
