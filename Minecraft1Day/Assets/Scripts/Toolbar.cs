using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Toolbar : MonoBehaviour
{
    public PlayerController player;
    public RectTransform highlight;
    public List<ItemSlot> itemSlots;

    public int slotIndex = 0;

    private void Start()
    {
        foreach (ItemSlot itemSlot in itemSlots)
        {
            itemSlot.icon.sprite = Map.instance.blocksDict[itemSlot.blockType].sprite;
            itemSlot.icon.gameObject.SetActive(true);
        }
        
        player.selectedBlockType = itemSlots[slotIndex].blockType;
    }

    private void Update()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll != 0)
        {
            if (scroll > 0)
            {
                slotIndex--;
            }
            else
            {
                slotIndex++;
            }

            if (slotIndex > itemSlots.Count - 1)
            {
                slotIndex = 0;
            }

            if (slotIndex < 0)
            {
                slotIndex = itemSlots.Count - 1; ;
            }
            UpdateHighlight();
        }
    }

    private void UpdateHighlight()
    {
        highlight.position = itemSlots[slotIndex].icon.transform.position;
        player.selectedBlockType = itemSlots[slotIndex].blockType;
    }
}

[Serializable]
public class ItemSlot
{
    public BlockType blockType;
    public Image icon;
}
