using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

public class UI_RewardItem : UI_Base
{
    public ItemData _item;
    public Image itemImage;
    public TextMeshProUGUI itemCountText;

    public void SetInfo(ItemData item, int itemCount)
    {
        itemImage.sprite = item.itemIcon;
        itemCountText.text = itemCount.ToString();
    }
}
