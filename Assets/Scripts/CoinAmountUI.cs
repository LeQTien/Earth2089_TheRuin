using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CoinAmountUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI goldText; // Kéo Text UI từ Inspector vào đây

    private void OnEnable()
    {
        GameManager.OnGoldChanged += UpdateGoldUI; // Đăng ký lắng nghe sự kiện
    }

    private void OnDisable()
    {
        GameManager.OnGoldChanged -= UpdateGoldUI; // Hủy đăng ký khi object bị disable
    }

    private void Start()
    {
        // Đảm bảo cập nhật UI khi bắt đầu (nếu cần)
        UpdateGoldUI(FindObjectOfType<GameManager>().GetPlayerGold());
    }

    private void UpdateGoldUI(int newGoldAmount)
    {
        goldText.text = "Gold: " + newGoldAmount.ToString(); // Cập nhật text hiển thị số vàng
    }
}
