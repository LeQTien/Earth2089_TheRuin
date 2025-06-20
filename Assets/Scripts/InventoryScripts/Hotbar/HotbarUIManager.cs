using UnityEngine;
using UnityEngine.UI;

public class HotbarUIManager : MonoBehaviour
{
    [Header("Các icon của slot vũ khí")]
    [SerializeField] private Image[] slotIcons; // Gán 4 Image của các slot từ left to right

    [Header("Sprite mặc định nếu không có vũ khí")]
    [SerializeField] private Sprite defaultIcon;

    [Header("Panel chỉ định slot đang chọn")]
    [SerializeField] private RectTransform selectorPanel;

    public int SlotCount => slotIcons.Length;

    /// <summary>
    /// Cập nhật icon hotbar dựa vào mảng vũ khí.
    /// </summary>
    public void UpdateHotbarUI(GameObject[] weapons)
    {
        for (int i = 0; i < slotIcons.Length; i++)
        {
            if (i < weapons.Length && weapons[i] != null)
            {
                WeaponUIData uiData = weapons[i].GetComponent<WeaponUIData>();
                if (uiData != null && uiData.WeaponIcon != null)
                {
                    slotIcons[i].sprite = uiData.WeaponIcon;
                    slotIcons[i].enabled = true;
                }
                else
                {
                    slotIcons[i].sprite = defaultIcon; // Không có icon vũ khí, có thể set sprite là null hoặc sprite transparent
                    slotIcons[i].enabled = true;
                }
            }
            else
            {
                slotIcons[i].sprite = defaultIcon; // Không có icon vũ khí, có thể set sprite là null hoặc sprite transparent
                slotIcons[i].enabled = true;
            }
        }
    }

    /// <summary>
    /// Đặt selectorPanel lên slot đang được chọn.
    /// </summary>
    public void HighlightSlot(int selectedIndex)
    {
        if (selectedIndex >= 0 && selectedIndex < slotIcons.Length && selectorPanel != null)
        {
            selectorPanel.SetParent(slotIcons[selectedIndex].transform, false);
            selectorPanel.anchoredPosition = Vector2.zero;
        }
    }
}
