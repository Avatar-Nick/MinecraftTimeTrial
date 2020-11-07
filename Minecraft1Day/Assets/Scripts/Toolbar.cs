using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Toolbar : MonoBehaviour
{
    public PlayerController player;
    public RectTransform highlight;
    public List<ItemSlotGraphics> itemSlots;

    public int slotIndex = 0;

    private void Start()
    {
        foreach (ItemSlotGraphics itemSlot in itemSlots)
        {
            ItemSlotData itemSlotData = new ItemSlotData(itemSlot, itemSlot.itemSlotData.blockType, 10);
            itemSlotData.itemSlotGraphics.gameObject.SetActive(true);
        }
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
        highlight.position = itemSlots[slotIndex].slotIcon.transform.position;
    }
}
