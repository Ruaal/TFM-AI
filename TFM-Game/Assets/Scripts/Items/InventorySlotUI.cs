using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlotUI : MonoBehaviour
{
    public Image iconImage;
    public TMP_Text quantityText;
    public Button useButton;

    private int index;
    private Inventory inventory;

    public void Setup(Inventory inv, int i)
    {
        inventory = inv;
        index = i;
        useButton.onClick.AddListener(UseItem);
    }

    public void UpdateSlot(InventorySlot slot)
    {
        if (slot.IsEmpty())
        {
            iconImage.enabled = false;
            quantityText.text = "";
        }
        else
        {
            iconImage.enabled = true;
            iconImage.sprite = slot.item.icon;
            quantityText.text = slot.quantity > 1 ? slot.quantity.ToString() : "";
        }
    }

    private void UseItem()
    {
        inventory.UseItem(index);
    }
}
