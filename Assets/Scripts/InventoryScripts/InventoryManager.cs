using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] private Canvas canvas; // canvas chứa itemCursor
    [SerializeField] private GameObject itemCursor;

    [SerializeField] private GameObject slotHolder;
    [SerializeField] private GameObject hotbarSlotHolder;

    [SerializeField] private ItemClass itemToAdd;
    [SerializeField] private ItemClass itemToAdd2;

    [SerializeField] private ItemClass itemToRemove;

    //public List<SlotClass> inventory = new List<SlotClass>();
    [SerializeField] private SlotClass[] startingItems;
    private SlotClass[] inventory;

    private GameObject[] slots;
    private GameObject[] hotbarSlots;

    private SlotClass movingSlot;
    private SlotClass tempSlot;
    private SlotClass originalSlot;
    bool isMovingItem;

    [SerializeField] private GameObject hotbarSelector;
    [SerializeField] private int selectedHotbarSlotIndex = 0;
    public ItemClass selectedHotbarItem;

    public void ToggleInventory()
    {
        if (gameObject.activeSelf)
        {
            gameObject.SetActive(false);
        }
        else
        {
            gameObject.SetActive(true);
        }
    }

    private void Start()
    {
        gameObject.SetActive(false);
        slots = new GameObject[slotHolder.transform.childCount];
        inventory = new SlotClass[slots.Length];

        hotbarSlots = new GameObject[hotbarSlotHolder.transform.childCount];
        for (int i = 0; i < hotbarSlotHolder.transform.childCount; i++)
        {
            hotbarSlots[i] = hotbarSlotHolder.transform.GetChild(i).gameObject;
        }

        for (int i = 0; i < inventory.Length; i++)
        {
            inventory[i] = new SlotClass();
        }
        for (int i = 0; i < startingItems.Length; i++)
        {
            inventory[i] = startingItems[i];
        }

        // Fill the slots array with the child objects of slotHolder
        for (int i = 0; i < slotHolder.transform.childCount; i++)
        {
            slots[i] = slotHolder.transform.GetChild(i).gameObject;
        }
        RefreshUI();
        // Example usage
        AddItem(itemToAdd, 10);
        AddItem(itemToAdd2, 1);

        RemoveItem(itemToRemove);
    }
    private void Update()
    {
        //itemCursor.SetActive(isMovingItem);
        //itemCursor.transform.position = Input.mousePosition;
        //Debug.Log("itemCursor position: " + itemCursor.transform.position);
        if (isMovingItem)
        {
            itemCursor.SetActive(true);

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.transform as RectTransform,
                Input.mousePosition,
                canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera,
                out Vector2 localPoint
            );
            itemCursor.GetComponent<Image>().sprite = movingSlot.GetItem().itemIcon;
            itemCursor.GetComponent<RectTransform>().localPosition = localPoint;
        }
        else
        {
            itemCursor.SetActive(false);
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (isMovingItem)
            {
                EndItemMove();
            }
            else
            {
                BeginItemMove();
            }
        }
        else if (Input.GetMouseButtonDown(1))
        {
            if (isMovingItem)
            {
                if (movingSlot.GetItem().isStackable)
                {
                    DropSingleItem();
                }
                else
                {
                    EndItemMove(); // Unstackable → di chuyển luôn
                }
            }
            else
            {
                SlotClass closest = GetClosestSlot();
                if (closest != null && closest.GetItem() != null)
                {
                    if (closest.GetItem().isStackable)
                    {
                        BeginItemMove_Half(); // chia nửa
                    }
                    else
                    {
                        BeginItemMove(); // lấy nguyên
                    }
                }
            }
        }

        if (Input.GetAxis("Mouse ScrollWheel") > 0f) // scroll up
        {
            selectedHotbarSlotIndex = Mathf.Clamp(selectedHotbarSlotIndex + 1, 0, hotbarSlots.Length - 1);
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0f) // scroll down
        {
            selectedHotbarSlotIndex = Mathf.Clamp(selectedHotbarSlotIndex - 1, 0, hotbarSlots.Length - 1);
            if (selectedHotbarSlotIndex < 0)
            {
                selectedHotbarSlotIndex = hotbarSlots.Length - 1;
            }
        }

        hotbarSelector.transform.position = hotbarSlots[selectedHotbarSlotIndex].transform.position;
        selectedHotbarItem = inventory[selectedHotbarSlotIndex + (hotbarSlots.Length * 3)].GetItem();
    }
    #region Inventory Utils
    public void RefreshUI()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            try
            {
                slots[i].transform.GetChild(0).GetComponent<Image>().enabled = true;
                slots[i].transform.GetChild(0).GetComponent<Image>().sprite = inventory[i].GetItem().itemIcon;

                if (inventory[i].GetItem().isStackable)
                    slots[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = inventory[i].GetQuantity() + "";

                else
                    slots[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "";
            }
            catch
            {
                slots[i].transform.GetChild(0).GetComponent<Image>().sprite = null;
                slots[i].transform.GetChild(0).GetComponent<Image>().enabled = false;
                slots[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "";
            }
        }
        RefreshHotbarUI();
    }
    public void RefreshHotbarUI()
    {
        for (int i = 0; i < hotbarSlots.Length; i++)
        {
            try
            {
                hotbarSlots[i].transform.GetChild(0).GetComponent<Image>().enabled = true;
                hotbarSlots[i].transform.GetChild(0).GetComponent<Image>().sprite = inventory[i + (hotbarSlots.Length * 3)].GetItem().itemIcon;

                if (inventory[i + (hotbarSlots.Length * 3)].GetItem().isStackable)
                    hotbarSlots[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = inventory[i + (hotbarSlots.Length * 3)].GetQuantity() + "";

                else
                    hotbarSlots[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "";
            }
            catch
            {
                hotbarSlots[i].transform.GetChild(0).GetComponent<Image>().sprite = null;
                hotbarSlots[i].transform.GetChild(0).GetComponent<Image>().enabled = false;
                hotbarSlots[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "";
            }
        }
    }
    public bool AddItem(ItemClass item, int quantity)
    {
        // Nếu item stackable, cố gắng thêm vào các slot đã có item đó trước
        if (item.isStackable)
        {
            for (int i = 0; i < inventory.Length; i++)
            {
                if (inventory[i].GetItem() == item)
                {
                    int currentQty = inventory[i].GetQuantity();
                    int spaceLeft = item.maxStack - currentQty;

                    if (spaceLeft > 0)
                    {
                        int addAmount = Mathf.Min(spaceLeft, quantity);
                        inventory[i].AddQuantity(addAmount);
                        quantity -= addAmount;

                        if (quantity <= 0)
                        {
                            RefreshUI();
                            return true;
                        }
                    }
                }
            }
        }

        // Thêm vào slot trống nếu còn dư
        for (int i = 0; i < inventory.Length; i++)
        {
            if (inventory[i].GetItem() == null)
            {
                int addAmount = item.isStackable ? Mathf.Min(quantity, item.maxStack) : 1;
                inventory[i].AddItem(item, addAmount);
                quantity -= addAmount;

                if (quantity <= 0)
                {
                    RefreshUI();
                    return true;
                }
            }
        }

        // Nếu vẫn còn quantity nhưng không còn slot
        if (quantity > 0)
        {
            Debug.LogWarning("Inventory is full. Could not add all items.");
            RefreshUI();
            return false;
        }

        RefreshUI();
        return true;
    }

    public bool RemoveItem(ItemClass item)
    {
        //if (inventory.Contains(item))
        //{
        //    inventory.Remove(item);
        //    Debug.Log("Removed " + item.itemName + " from inventory.");
        //}
        //else
        //{
        //    Debug.Log("Item not found in inventory.");
        //}
        SlotClass temp = Contains(item);
        if (temp != null)
        {
            if (temp.GetQuantity() > 1)
                temp.SubQuantity(1);
            else
            {
                int slotToRemoveIndex = 0;
                for (int i = 0; i < inventory.Length; i++)
                {
                    if (inventory[i].GetItem() == item)
                    {
                        slotToRemoveIndex = i;
                        break;
                    }
                }
            }
        }
        else
        {
            return false;
        }
        RefreshUI();
        return true;
    }
    public void UseSelectedItem()
    {
        inventory[selectedHotbarSlotIndex + (hotbarSlots.Length * 3)].SubQuantity(1);
        RefreshUI();
    }
    public SlotClass Contains(ItemClass item)
    {
        //foreach (SlotClass slot in inventory)
        //{
        //    if (slot.GetItem() == item)
        //    {
        //        return slot;
        //    }
        //}
        for (int i = 0; i < inventory.Length; i++)
        {
            if (inventory[i].GetItem() == item)
            {
                return inventory[i];
            }
        }
        return null;
    }
    #endregion Inventory Utils

    #region Moving Items
    private bool BeginItemMove()
    {
        originalSlot = GetClosestSlot();
        if (originalSlot == null || originalSlot.GetItem() == null)
        {
            return false; // No item to move
        }
        movingSlot = new SlotClass(originalSlot);
        originalSlot.Clear();
        isMovingItem = true;
        RefreshUI();
        return true;
    }
    private bool BeginItemMove_Half()
    {
        originalSlot = GetClosestSlot();
        if (originalSlot == null || originalSlot.GetItem() == null)
        {
            return false; // No item to move
        }

        // 💥 Kiểm tra item có stackable không
        if (!originalSlot.GetItem().isStackable || originalSlot.GetQuantity() < 2)
        {
            return false; // Không thể chia item không stack được, hoặc chỉ có 1
        }

        int halfAmount = Mathf.CeilToInt(originalSlot.GetQuantity() / 2f);

        movingSlot = new SlotClass(originalSlot.GetItem(), halfAmount);
        originalSlot.SubQuantity(halfAmount);

        isMovingItem = true;
        RefreshUI();
        return true;
    }
    private void DropSingleItem()
    {
        SlotClass closest = GetClosestSlot();
        if (closest == null || movingSlot.GetItem() == null) return;

        if (closest.GetItem() == null)
        {
            // Ô trống → thêm 1 item
            closest.AddItem(movingSlot.GetItem(), 1);
            movingSlot.SubQuantity(1);
        }
        else if (closest.GetItem() == movingSlot.GetItem() && movingSlot.GetItem().isStackable)
        {
            int maxStack = closest.GetItem().maxStack;
            if (closest.GetQuantity() < maxStack)
            {
                closest.AddQuantity(1);
                movingSlot.SubQuantity(1);
            }
            else
            {
                // Không thêm được vì đã đầy
                return;
            }
        }

        else
        {
            // Swap nếu khác loại
            SlotClass tempSlot = new SlotClass(closest);
            closest.Clear();
            closest.AddItem(movingSlot.GetItem(), movingSlot.GetQuantity());
            movingSlot.Clear();
            movingSlot.AddItem(tempSlot.GetItem(), tempSlot.GetQuantity());
        }

        if (movingSlot.GetQuantity() <= 0)
        {
            movingSlot.Clear();
            isMovingItem = false;
        }

        RefreshUI();
    }

    private bool EndItemMove()
    {
        originalSlot = GetClosestSlot();
        if (originalSlot == null)
        {
            AddItem(movingSlot.GetItem(), movingSlot.GetQuantity());
            movingSlot.Clear();
        }
        else
        {

            if (originalSlot.GetItem() != null)
            {
                if (originalSlot.GetItem() == movingSlot.GetItem()) // there is already an item in the slot
                {
                    if (originalSlot.GetItem().isStackable)
                    {
                        int maxStack = originalSlot.GetItem().maxStack;
                        int availableSpace = maxStack - originalSlot.GetQuantity();

                        if (availableSpace >= movingSlot.GetQuantity())
                        {
                            originalSlot.AddQuantity(movingSlot.GetQuantity());
                            movingSlot.Clear();
                        }
                        else if (availableSpace > 0)
                        {
                            originalSlot.AddQuantity(availableSpace);
                            movingSlot.SubQuantity(availableSpace);
                            return false; // còn dư → vẫn giữ itemCursor
                        }
                        else
                        {
                            // không còn chỗ → không làm gì
                            return false;
                        }
                    }
                    else
                    {
                        // do nothing, just return
                        return false;
                    }
                }
                else
                {
                    tempSlot = new SlotClass(originalSlot); // lưu item cũ nếu cần

                    originalSlot.AddItem(movingSlot.GetItem(), movingSlot.GetQuantity()); // đặt item mới
                    movingSlot.AddItem(tempSlot.GetItem(), tempSlot.GetQuantity()); // đặt item cũ vào slot đang kéo

                    RefreshUI();
                    return true;
                }
            }
            else // place item as usual
            {
                originalSlot.AddItem(movingSlot.GetItem(), movingSlot.GetQuantity());
                movingSlot.Clear();
            }

        }
        isMovingItem = false;
        RefreshUI();
        return true;
    }

    private SlotClass GetClosestSlot()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            Vector2 slotScreenPos = Camera.main.WorldToScreenPoint(slots[i].transform.position);
            if (Vector2.Distance(slotScreenPos, Input.mousePosition) <= 32)
            {
                return inventory[i];
            }
        }
        return null;
    }
    #endregion Moving Items
}