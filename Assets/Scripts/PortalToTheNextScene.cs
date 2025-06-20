using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PortalToTheNextScene : MonoBehaviour
{
    private bool isActivated = false;
    public string sceneToLoad = "SceneLevel2";
    private AbstractMeleeWeapon meleeWeapon;
    [SerializeField] private GameObject LoadingScreen;

    private MultiZoneGenerator2 multiZoneGenerator2;

    // Gọi hàm này từ vũ khí khi chém trúng
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isActivated) return;

        if (collision.CompareTag("MeleeWeapon"))
        {
            AbstractMeleeWeapon meleeWeapon = collision.GetComponent<AbstractMeleeWeapon>();

            if (meleeWeapon != null && meleeWeapon.GetIsAttacking())
            {
                isActivated = true;
                Debug.Log("Portal activated by sword!");
                LoadingScreen.SetActive(true);
                SceneManager.LoadScene(sceneToLoad);
            }
        }
    }
}
