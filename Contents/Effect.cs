using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect : MonoBehaviour
{
    void OnEnable() { GetComponent<ParticleSystem>().Play(); }
    void OnDisable() { GetComponent<ParticleSystem>().Stop(); }
}
