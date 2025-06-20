using UnityEngine;

public class Gold : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            GameManager gameManager = FindObjectOfType<GameManager>();
            if (gameManager != null)
            {
                // Call the AddGold method on the GameManager instance
                gameManager.AddGold(1);

                // Optional: sound, effects...

                // Destroy the gold object after being picked up
                Destroy(gameObject);
            }
            else
            {
                Debug.LogError("GameManager instance not found!");
            }
        }
    }
}
