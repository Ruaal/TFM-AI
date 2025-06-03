using System;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public int capacity = 20;
    public InventorySlot[] slots;
    public event Action OnInventoryChanged;

    void Awake()
    {
        slots = new InventorySlot[capacity];
        for (int i = 0; i < capacity; i++)
            slots[i] = new InventorySlot();
    }

    public bool AddItem(ItemData newItem, int amount = 1)
    {
        for (int i = 0; i < capacity; i++)
        {
            if (slots[i].CanStack(newItem))
            {
                slots[i].quantity += amount;
                OnInventoryChanged?.Invoke();
                return true;
            }
        }
        for (int i = 0; i < capacity; i++)
        {
            if (slots[i].IsEmpty())
            {
                slots[i].item = newItem;
                slots[i].quantity = amount;
                OnInventoryChanged?.Invoke();
                return true;
            }
        }
        return false;
    }

    public void RemoveItem(int slotIndex, int amount = 1)
    {
        if (slotIndex < 0 || slotIndex >= capacity)
            return;
        var slot = slots[slotIndex];
        if (slot.IsEmpty())
            return;
        slot.quantity -= amount;
        if (slot.quantity <= 0)
        {
            slot.item = null;
            slot.quantity = 0;
        }
        OnInventoryChanged?.Invoke();
    }

    public void UseItem(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= capacity)
            return;
        var slot = slots[slotIndex];
        if (slot.IsEmpty())
            return;
        RemoveItem(slotIndex);
    }
}
