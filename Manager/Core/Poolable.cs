using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * File :   Poolable.cs
 * Desc :   Poolable 스크립트가 객체에 컴포넌트로 추가되어 있는지 확인하여 Pool 여부를 체크한다.
 *          [ Rookiss의 MMORPG Game Part 3 참고. ]
 */

public class Poolable : MonoBehaviour
{
    public bool IsUsing;    // 아무것도 없어 허전하니 풀 중인지 알려주는 변수를 넣어주었다.
}
