using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/*
[ 가이드 UI 스크립트 ]
1. Level Up, 돈 부족 등.. 상황에 띄울 수 있는 Text UI이다.
2. 자주 호출되는 함수 : SetInfo()
*/

public class UI_Guide : UI_Base
{
    [SerializeField]
    TextMeshProUGUI _messageText;

    Color _color;

    Coroutine co;

    public void SetInfo(string messageText, Color color)
    {
        _messageText.text = messageText;
        _messageText.transform.localPosition = Vector3.zero;
        _color = color;
        _messageText.color = _color;

        if (co.IsNull() == false) StopCoroutine(co);
        co = StartCoroutine(MessageCoroutine());
    }

    IEnumerator MessageCoroutine()
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
