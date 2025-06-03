using System;
using System.Collections;
using UnityEngine;

public class Collectable : MonoBehaviour
{
    [SerializeField]
    private ItemData itemData;

    [SerializeField]
    private float respawnTime = 5f;

    private int quantity = 1;

    public static event Action<string, int> OnItemCollected;

    private Collider[] colliders;
    private MeshRenderer[] meshRenderers;
    private Transform[] childObjects;

    private void Awake()
    {
        colliders = GetComponentsInChildren<Collider>();
        meshRenderers = GetComponentsInChildren<MeshRenderer>();

        // Excluye el objeto raíz
        childObjects = new Transform[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            childObjects[i] = transform.GetChild(i);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        var inventory = other.GetComponent<Inventory>();
        if (inventory != null && inventory.AddItem(itemData, quantity))
        {
            OnItemCollected?.Invoke(itemData.itemName, quantity);
            StartCoroutine(HandleRespawn());
        }
    }

    private IEnumerator HandleRespawn()
    {
        SetComponentsActive(false);
        SetChildrenActive(false);

        yield return new WaitForSeconds(respawnTime);

        SetComponentsActive(true);
        SetChildrenActive(true);
    }

    private void SetComponentsActive(bool active)
    {
        foreach (var col in colliders)
            col.enabled = active;

        foreach (var rend in meshRenderers)
            rend.enabled = active;
    }

    private void SetChildrenActive(bool active)
    {
        foreach (var child in childObjects)
        {
            if (child != null)
                child.gameObject.SetActive(active);
        }
    }
}
