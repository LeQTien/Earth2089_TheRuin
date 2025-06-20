//using UnityEngine;

//public class CursorManager : MonoBehaviour
//{
//    [SerializeField] private Texture2D cursorNormal;
//    [SerializeField] private Texture2D cursorShoot;
//    [SerializeField] private Texture2D cursorReload;

//    public GameObject gunObject;
//    public GameObject shotGunObject;

//    // biến điều chỉnh vị trí con trỏ chuột, 16 và 48 là vị trí hình ảnh con trỏ chuột đối với con trỏ chuột ban đầu
//    private Vector2 cursorHotspot = new(0, 0);
//    private Texture2D lastCursor; // biến theo dõi con trỏ hiện tại

//    void Start()
//    {
//        // thiết lập con trỏ chuột mặc định, cursorHotspot là vị trí chuột, CursorMode.Auto là thiết lập kiểu con trỏ tự động
//        Cursor.SetCursor(cursorNormal, cursorHotspot, CursorMode.Auto);
//    }

//    void Update()
//    {
//        CursorDisplay();
//    }

//    void CursorDisplay()
//    {
//        Gun theGun = gunObject.GetComponent<Gun>();
//        ShotGun theShotGun = shotGunObject.GetComponent<ShotGun>();

//        Texture2D targetCursor = cursorNormal;

//        if (theGun.IsReloading())
//        {
//            targetCursor = cursorReload;
//        }
//        else if (theGun.IsShooting())
//        {
//            targetCursor = cursorShoot;
//        }

//        if (theShotGun.IsReloading())
//        {
//            targetCursor = cursorReload;
//        }
//        else if (theShotGun.IsShooting())
//        {
//            targetCursor = cursorShoot;
//        }

//        if (lastCursor != targetCursor) // chỉ đổi nếu con trỏ tiếp theo khác con trỏ hiện tại
//        {
//            Cursor.SetCursor(targetCursor, cursorHotspot, CursorMode.Auto);
//            lastCursor = targetCursor;
//        }
//    }
//}
