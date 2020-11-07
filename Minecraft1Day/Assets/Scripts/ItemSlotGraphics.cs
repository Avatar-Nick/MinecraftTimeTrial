using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlotGraphics : MonoBehaviour
{
    [Header("Data")]
    public ItemSlotData itemSlotData;

    [Header("UI")]
    public Image slotIcon;
    public TextMeshProUGUI amountText;

    public void Link(ItemSlotData itemSlotData)
    {
        this.itemSlotData = itemSlotData;
        this.itemSlotData.itemSlotGraphics = this;
        UpdateSlot();
    }

    public void UnLink()
    {
        this.itemSlotData.itemSlotGraphics = null;
        this.itemSlotData = null;
        UpdateSlot();
    }

    public void UpdateSlot()
    {
        if (itemSlotData != null)
        {
            slotIcon.sprite = Map.instance.blocksDict[itemSlotData.blockType].sprite;
            slotIcon.gameObject.SetActive(true);
            amountText.text = itemSlotData.amount.ToString();
            amountText.gameObject.SetActive(true);
        }
        else
        {
            Clear();
        }
    }

    public void Clear()
    {
        slotIcon.sprite = null;
        amountText.text = "";
        slotIcon.gameObject.SetActive(false);
        amountText.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        if (itemSlotData != null)
        {
            itemSlotData.UnLink();
        }
    }
}

[Serializable]
public class ItemSlotData
{
    [Header("Data")]
    public BlockType blockType;
    public int amount;

    [Header("UI")]
    public ItemSlotGraphics itemSlotGraphics;

    public ItemSlotData(ItemSlotGraphics itemSlot)
    {
        Link(itemSlotGraphics);
        itemSlot.Link(this);

        itemSlotGraphics.UpdateSlot();
    }

    public ItemSlotData(ItemSlotGraphics itemSlot, BlockType blockType, int amount)
    {
        this.blockType = blockType;
        this.amount = amount;

        Link(itemSlotGraphics);
        itemSlot.Link(this);

        itemSlotGraphics.UpdateSlot();
    }

    public void Link(ItemSlotGraphics itemSlotGraphics)
    {
        this.itemSlotGraphics = itemSlotGraphics;
    }

    public void UnLink()
    {
        itemSlotGraphics = null;
    }

    public int Remove(int amount)
    {
        if (amount >= this.amount)
        {
            int amt = this.amount;
            this.amount = 0;
            itemSlotGraphics.UpdateSlot();
            return amt;
        }

        this.amount -= amount;
        itemSlotGraphics.UpdateSlot();
        return amount;
    }
}
