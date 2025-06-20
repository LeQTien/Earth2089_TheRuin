using UnityEngine;

public class ItemClass : ScriptableObject
{
    [Header("Item Info")] // data shared across every items
    public string itemName;
    public Sprite itemIcon;
    public bool isStackable;
    public int maxStack = 12;

    public virtual void Use(Player caller)
    {
        // default use function
        // this function will be called when the item is used
        // it will be overridden by the child classes
        Debug.Log("Using item: " + itemName);
    }
    public virtual ItemClass GetItem() { return this; } // return the item itself
    public virtual ToolClass GetTool() { return null; } // return null if not a tool
    public virtual ConsumableClass GetConsumable() { return null; } // return null if not a consumable
}
