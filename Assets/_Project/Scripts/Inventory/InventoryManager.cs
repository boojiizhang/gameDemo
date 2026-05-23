using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 道具栏管理器。
/// 挂载位置：GameManagers.prefab / InventoryManager。
/// </summary>
public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    private readonly List<ItemData> collectedItems = new List<ItemData>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;
    }

    private void OnEnable()
    {
        GameEvents.OnItemScanned += AddItem;
    }

    private void OnDisable()
    {
        GameEvents.OnItemScanned -= AddItem;
    }

    public void AddItem(ItemData itemData)
    {
        if (itemData == null)
        {
            Debug.LogWarning("[InventoryManager] 道具数据为空，无法加入道具栏。");
            return;
        }

        if (HasItem(itemData))
        {
            Debug.Log("[InventoryManager] 已经拥有道具：" + itemData.itemName);
            return;
        }

        collectedItems.Add(itemData);
        GameEvents.RaiseItemAdded(itemData);
    }

    public bool HasItem(ItemData itemData)
    {
        if (itemData == null)
        {
            return false;
        }

        for (int i = 0; i < collectedItems.Count; i++)
        {
            if (collectedItems[i] != null && collectedItems[i].itemId == itemData.itemId)
            {
                return true;
            }
        }

        return false;
    }

    public List<ItemData> GetCollectedItems()
    {
        return new List<ItemData>(collectedItems);
    }

    public void ResetInventory()
    {
        collectedItems.Clear();
    }
}
