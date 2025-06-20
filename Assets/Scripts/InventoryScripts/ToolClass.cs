using UnityEngine;

[CreateAssetMenu(fileName = "New Tool Class", menuName = "Item/Tool")]
public class ToolClass : ItemClass
{
    [Header("Tool Info")]
    public ToolType toolType;
    public enum ToolType
    {
        meleeWeapon,
        tool
    }
    //public override void Use(Player caller)
    //{
    //    base.Use(caller);
    //    Debug.Log("Swing Tool: " + itemName);
    //}
    public override void Use(Player caller)
    {
        base.Use(caller);
        Debug.Log("Swing Tool: " + itemName);

        //// Nếu là vũ khí cận chiến, yêu cầu player thực hiện tấn công
        //if (toolType == ToolType.meleeWeapon)
        //{
        //    caller.HandleAttack(); // bạn sẽ thêm hàm này ở Player
        //}
    }


    public override ToolClass GetTool() { return this; }
}
