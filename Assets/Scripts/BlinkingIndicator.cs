using System.Collections;
using UnityEngine;

public class BlinkingIndicator : MonoBehaviour
{
    [SerializeField] private float blinkInterval = 0.2f; // thời gian giữa các lần chớp
    [SerializeField] private float duration = 1.5f; // tổng thời gian hiệu ứng

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        StartCoroutine(BlinkAndDestroy());
    }

    private IEnumerator BlinkAndDestroy()
    {
        float elapsed = 0f; // đếm tổng thời gian đã trôi qua từ lúc hiệu ứng bắt đầu
        bool visible = true; // theo dõi trạng thái hiện tại của sprite

        while (elapsed < duration)
        {
            visible = !visible; // Mỗi lần lặp, trạng thái visible bị đảo ngược
            spriteRenderer.enabled = visible; // Gán trạng thái vừa đảo ngược vào spriteRenderer.enabled

            yield return new WaitForSeconds(blinkInterval); // Dừng coroutine lại trong blinkInterval giây (ví dụ 0.2s) trước khi tiếp tục vòng lặp tiếp theo
            elapsed += blinkInterval; // Cập nhật tổng thời gian đã trôi qua
        }

        Destroy(gameObject); // tự hủy sau khi hoàn tất chớp nháy
    }
}