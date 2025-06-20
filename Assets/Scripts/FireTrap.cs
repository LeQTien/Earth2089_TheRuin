using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireTrap : MonoBehaviour
{
    [Header("Thời gian hoạt động")]
    private float idleDuration;
    private float activateDuration;
    [SerializeField] private AnimationClip idleClip;
    [SerializeField] private AnimationClip activateClip;

    [Header("Sát thương")]
    [SerializeField] private float enterDamage = 20f;
    [SerializeField] private float damageCooldown = 1f;

    private Animator animator;
    private Collider2D trapCollider;
    private bool isActive = false;

    // Cooldown mỗi player
    private Dictionary<GameObject, float> damageCooldowns = new Dictionary<GameObject, float>();

    private void Start()
    {
        animator = GetComponent<Animator>();
        trapCollider = GetComponent<Collider2D>();

        idleDuration = idleClip.length;
        activateDuration = activateClip.length;

        if (trapCollider == null)
        {
            Debug.LogError("Thiếu Collider2D trên FireTrap.");
            enabled = false;
            return;
        }

        trapCollider.enabled = false;
        StartCoroutine(TrapCycle());
    }

    private IEnumerator TrapCycle()
    {
        while (true)
        {
            // Idle phase
            isActive = false;
            trapCollider.enabled = false;
            animator.SetTrigger("Idle");
            yield return new WaitForSeconds(idleDuration);

            // Activate phase
            isActive = true;
            trapCollider.enabled = true;
            animator.SetTrigger("Activate");
            yield return new WaitForSeconds(activateDuration);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isActive) return;
        if (!other.CompareTag("Player")) return;

        Player player = other.GetComponent<Player>();
        if (player != null)
        {
            player.TakeDamage(enterDamage);
            Debug.Log("Player bị trúng bẫy khi vừa bước vào!");
            damageCooldowns[other.gameObject] = Time.time + damageCooldown;
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!isActive) return;
        if (!other.CompareTag("Player")) return;

        GameObject playerObj = other.gameObject;

        if (damageCooldowns.TryGetValue(playerObj, out float cooldownEndTime))
        {
            if (Time.time < cooldownEndTime) return;
        }

        Player player = playerObj.GetComponent<Player>();
        if (player != null)
        {
            player.TakeDamage(enterDamage);
            Debug.Log("Player bị đốt khi đứng trong trap!");
            damageCooldowns[playerObj] = Time.time + damageCooldown;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (damageCooldowns.ContainsKey(other.gameObject))
        {
            damageCooldowns.Remove(other.gameObject);
        }
    }
}
