using UnityEngine;

public class AbstractGunClass : MonoBehaviour
{
    protected float rotateOffset = 180f; // góc offset khi xoay khẩu súng
    [SerializeField] protected GameObject bulletPrefab; // viên đạn

    [SerializeField] protected AudioManager audioManager;

    //[SerializeField] protected GameObject gunGameObject;
    //[SerializeField] protected GameObject shotGunGameObject;

    protected float baseFireRate;
    protected float baseFireRate_Static = 0.1f;
    protected GameManager gameManager;
    protected Player player;

    protected bool lastRageState;

    

    protected virtual void Start()
    {
        gameManager = FindAnyObjectByType<GameManager>();
        player = FindAnyObjectByType<Player>();
        baseFireRate = baseFireRate_Static;
    }
    //protected virtual void Update()
    //{
    //    bool isRageActive = gameManager.IsRageActive();
    //    if (isRageActive != lastRageState)
    //    {
    //        ApplyRageModeFireRateStats();
    //        lastRageState = isRageActive;
    //    }
    //}

    protected virtual void RotateGun()
    {
        // kiểm tra xem chuột có trong màn hình hay không
        if (Input.mousePosition.x < 0 || Input.mousePosition.x > Screen.width || Input.mousePosition.y < 0 || Input.mousePosition.y > Screen.height)
        {
            return;
        }

        // xác định hướng từ vị trí khẩu súng đến vị trí con trỏ chuột
        Vector3 displacement = transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition);
        // tính góc xuay từ vị trí khẩu súng đến vị trí con trỏ chuột, chuyển đổi từ radian sang độ
        float angle = Mathf.Atan2(displacement.y, displacement.x) * Mathf.Rad2Deg;

        // xoay khẩu súng theo trục z với góc offset
        transform.rotation = Quaternion.Euler(0, 0, angle + rotateOffset);

        // sử dụng biến angle để lật hình ảnh khẩu súng theo trục y
        if (angle < -90 || angle > 90)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else
        {
            transform.localScale = new Vector3(1, -1, 1);
        }
    }
}
