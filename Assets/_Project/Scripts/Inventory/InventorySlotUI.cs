using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 单个道具格子 UI。
/// 挂载位置：InventorySlotUI.prefab。
/// </summary>
public class InventorySlotUI : MonoBehaviour
{
    [Header("UI 组件")]
    public Image backgroundImage;
    public Image iconImage;
    public Text iconText;
    public Text nameText;
    public Text stateText;

    [Header("颜色")]
    public Color ownedColor = new Color(1f, 0.9f, 0.25f, 1f);
    public Color missingColor = new Color(0.35f, 0.35f, 0.35f, 1f);

    private ItemData currentItem;

    public void Refresh(ItemData itemData, bool owned)
    {
        currentItem = itemData;

        if (nameText != null)
        {
            nameText.text = itemData != null ? itemData.itemName : "未知道具";
        }

        RefreshIcon();
        SetOwnedState(owned);
    }

    public void RefreshOwnedState()
    {
        bool owned = InventoryManager.Instance != null && InventoryManager.Instance.HasItem(currentItem);
        SetOwnedState(owned);
    }

    private void RefreshIcon()
    {
        if (currentItem != null && currentItem.icon != null)
        {
            if (iconImage != null)
            {
                iconImage.sprite = currentItem.icon;
                iconImage.enabled = true;
            }

            if (iconText != null)
            {
                iconText.gameObject.SetActive(false);
            }
        }
        else
        {
            if (iconImage != null)
            {
                iconImage.enabled = false;
            }

            if (iconText != null)
            {
                iconText.gameObject.SetActive(true);
                iconText.text = GetPlaceholderText();
            }
        }
    }

    private string GetPlaceholderText()
    {
        if (currentItem == null || string.IsNullOrEmpty(currentItem.itemName))
        {
            return "?";
        }

        return currentItem.itemName.Substring(0, 1);
    }

    private void SetOwnedState(bool owned)
    {
        if (backgroundImage != null)
        {
            backgroundImage.color = owned ? ownedColor : missingColor;
        }

        if (stateText != null)
        {
            stateText.text = owned ? "已获得" : "未获得";
        }
    }
}
