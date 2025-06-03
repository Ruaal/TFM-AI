using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    public Inventory inventory;
    public InventorySlotUI slotPrefab;
    public Transform slotContainer;

    private InventorySlotUI[] slotUIs;

    private void OnEnable()
    {
        inventory.OnInventoryChanged += RefreshUI;
    }

    private void OnDisable()
    {
        inventory.OnInventoryChanged -= RefreshUI;
    }

    void Start()
    {
        for (int i = 0; i < inventory.capacity; i++)
        {
            var slotUI = Instantiate(slotPrefab, slotContainer);
            slotUI.Setup(inventory, i);
            slotUIs[i] = slotUI;
        }
        RefreshUI();
    }

    public void RefreshUI()
    {
        if (slotUIs == null)
        {
            slotUIs = new InventorySlotUI[inventory.capacity];
        }
        for (int i = 0; i < inventory.capacity; i++)
        {
            slotUIs[i].UpdateSlot(inventory.slots[i]);
        }
    }
}
