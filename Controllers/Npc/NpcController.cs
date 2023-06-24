using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class NpcController : BaseController
{
    [SerializeField] protected string npcName;
    [SerializeField] protected float scanRange;

    protected UI_NameBar nameBarUI = null;

    public override void Init()
    {
        nameBarUI = Managers.UI.MakeWorldSpaceUI<UI_NameBar>(transform);
        nameBarUI.nameText = npcName;
    }

    protected override void UpdateIdle()
    {
        Vector3 dir = Managers.Game.GetPlayer().transform.position - transform.position;

        if (dir.magnitude <= scanRange)
        {
            OnInteract();

            _lockTarget = Managers.Game.GetPlayer();
            nameBarUI.gameObject.SetActive(true);

            dir.y = 0;
            transform.rotation = Quaternion.LookRotation(dir);
        }
        else
        {
            _lockTarget = null;
            nameBarUI.gameObject.SetActive(false);
        }
    }

    // 플레이어가 가까이 있다면 상호작용 가능
    void OnInteract()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            Managers.Game.IsInteract = !Managers.Game.IsInteract;

            Interact();
        }

        if (Managers.Game.IsInteract == true)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Managers.Game.IsInteract = false;
                Interact();
            }
        }
    }

    // 상호작용
    public abstract void Interact();
}
