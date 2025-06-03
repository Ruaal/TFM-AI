[System.Serializable]
public class InventorySlot
{
    public ItemData item;
    public int quantity;

    public bool IsEmpty() => item == null;

    public bool CanStack(ItemData newItem)
    {
        return !IsEmpty() && item == newItem && quantity < item.maxStack;
    }
}
