using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * File :   Effect.cs
 * Desc :   Effect 관련 스크립트의 부모이다.
 */

public class Effect : MonoBehaviour
{
    void OnEnable() { GetComponent<ParticleSystem>().Play(); }
    void OnDisable() { GetComponent<ParticleSystem>().Stop(); }
}
