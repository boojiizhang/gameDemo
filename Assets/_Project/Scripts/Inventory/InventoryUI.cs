using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 道具栏 UI。
/// 挂载位置：ARScanScene / Canvas_AR / InventoryUI。
/// </summary>
public class InventoryUI : MonoBehaviour
{
    [Header("格子设置")]
    public Transform slotRoot;
    public InventorySlotUI slotPrefab;

    private readonly List<InventorySlotUI> slots = new List<InventorySlotUI>();

    private void OnEnable()
    {
        GameEvents.OnTaskAccepted += BuildSlots;
        GameEvents.OnItemAdded += HandleItemAdded;
        RefreshFromManager();
    }

    private void OnDisable()
    {
        GameEvents.OnTaskAccepted -= BuildSlots;
        GameEvents.OnItemAdded -= HandleItemAdded;
    }

    private void RefreshFromManager()
    {
        if (TaskManager.Instance != null && TaskManager.Instance.HasAcceptedTask())
        {
            BuildSlots(TaskManager.Instance.GetCurrentTask());
        }
    }

    private void BuildSlots(TaskData taskData)
    {
        ClearSlots();

        if (taskData == null || taskData.requiredItems == null)
        {
            return;
        }

        for (int i = 0; i < taskData.requiredItems.Count; i++)
        {
            ItemData itemData = taskData.requiredItems[i];
            if (itemData == null || slotPrefab == null || slotRoot == null)
            {
                continue;
            }

            InventorySlotUI slot = Instantiate(slotPrefab, slotRoot);
            bool owned = InventoryManager.Instance != null && InventoryManager.Instance.HasItem(itemData);
            slot.Refresh(itemData, owned);
            slots.Add(slot);
        }
    }

    private void HandleItemAdded(ItemData itemData)
    {
        RefreshSlots();
    }

    private void RefreshSlots()
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i] != null)
            {
                slots[i].RefreshOwnedState();
            }
        }
    }

    private void ClearSlots()
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i] != null)
            {
                Destroy(slots[i].gameObject);
            }
        }

        slots.Clear();
    }
}
