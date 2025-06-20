using UnityEngine;

[CreateAssetMenu(fileName = "New Consumable Class", menuName = "Item/Consumable")]
public class ConsumableClass : ItemClass
{
    [Header("Consumable Info")]
    public float restoredValue;
    public override void Use(Player caller)
    {
        base.Use(caller);
        Debug.Log("Using Consumable: " + itemName);
        caller.inventory.UseSelectedItem();  // remove the item from the inventory
    }
    public override ConsumableClass GetConsumable() { return this; }
}
