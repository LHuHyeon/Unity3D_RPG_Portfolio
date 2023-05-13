using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackCollistion : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Monster"))
        {
            Debug.Log("Monster Hit!");
            other.GetComponent<Stat>().OnAttacked();
            Invoke("DelayActiveFalse", 0.1f);
        }
    }

    void DelayActiveFalse()
    {
        gameObject.SetActive(false);
    }
}
