using UnityEngine;

public class WalkerObject
{
    public Vector2 Position;
    public Vector2 Direction;
    public float ChanceToChange;

    public WalkerObject(Vector2 pos, Vector2 dir, float chanceToChange)
    {
        Position = pos;
        Direction = dir;
        ChanceToChange = chanceToChange;
    }

    // Cập nhật vị trí của Walker theo hướng di chuyển
    public void UpdatePosition()
    {
        Position += Direction;
    }

    // Thay đổi hướng di chuyển của Walker (tùy theo các điều kiện)
    public void ChangeDirection(Vector2 newDirection)
    {
        Direction = newDirection;
    }

    // Thay đổi xác suất thay đổi hướng di chuyển
    public void UpdateChanceToChange(float newChance)
    {
        ChanceToChange = newChance;
    }
}