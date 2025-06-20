using UnityEngine;

public class Spear : AbstractMeleeWeapon
{
    [SerializeField] protected float spearDamageMultiplier = 2f;
    [SerializeField] protected float spearAttackDelayMultiplier = 4f;
    [SerializeField] protected float spearKnockbackForce = 1.2f;
    [SerializeField] protected float spearStunDuration = 3f;

    [SerializeField] private Sprite weaponIcon;
    public Sprite WeaponIcon => weaponIcon;

    protected override void Start()
    {
        base.Start();
        UpdateRageStats();
    }

    protected override void Update()
    {
        base.Update();
        UpdateRageStats();
    }

    private void UpdateRageStats()
    {
        bool isRageActive = gameManager.IsRageActive();
        float rageMultiplier = isRageActive ? player.GetRageDamageMultiplier() : 1f;
        float fireRateDivider = isRageActive ? player.GetRageFireRateDivider() : 1f;
        float rageKnockbackForceMultiplier = isRageActive ? player.GetRageKnockbackForceMultiplier() : 1f;
        float rageStunDurationMultiplier = isRageActive ? player.GetRageStunDurationMultiplier() : 1f;

        attackDelay = baseAttackDelay * spearAttackDelayMultiplier / fireRateDivider;
        damage = baseDamage * spearDamageMultiplier * rageMultiplier;
        //attackingDuration = baseAttackingDuration / fireRateDivider;
        meleeWeaponKnockbackForce = baseMeleeWeaponKnockbackForce * spearKnockbackForce * rageKnockbackForceMultiplier;
        meleeWeaponStunDuration = baseMeleeWeaponStunDuration * spearStunDuration * rageStunDurationMultiplier;

    }
}
