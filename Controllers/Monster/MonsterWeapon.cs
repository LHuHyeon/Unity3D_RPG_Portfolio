using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterWeapon : MonoBehaviour
{
    public TrailRenderer trailRenderer;

    public void OnTrailRenderer(bool isTrue)
    {
        trailRenderer.enabled = isTrue;
    }
}
