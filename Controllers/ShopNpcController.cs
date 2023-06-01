using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopNpcController : NpcController
{
    public Define.ShopType shopType = Define.ShopType.Unknown;

    [SerializeField] private int shopBuyId;

    public override void Init()
    {
        base.Init();
    }

    public override void Interact()
    {

    }
}
