using UnityEngine;
using MoreMountains.Tools;
using MoreMountains.InventoryEngine;
using MoreMountains.CorgiEngine;
using System.Linq;
using System;

public class AutoEquipWeapon : MonoBehaviour
{
    InventoryInputManager inventoryInputManager;
    Inventory inventory;

    private void Start()
    {
        inventoryInputManager = SkillManager.Instance.inventoryInputManager;
        inventory = SkillManager.Instance.inventory;

    }

    private void OnDisable()
    {
        inventoryInputManager.OpenInventory();
        InventoryPickableItem picked = gameObject.GetComponent<InventoryPickableItem>();
        InventoryItem findItem = inventory.Content.FirstOrDefault(item => item.ItemID == picked.Item.ItemID);
        int idx = Array.FindIndex(inventory.Content, item => item.ItemID == picked.Item.ItemID);
        inventory.EquipItem(findItem, idx, null);
        inventoryInputManager.CloseInventory();
    }
}
