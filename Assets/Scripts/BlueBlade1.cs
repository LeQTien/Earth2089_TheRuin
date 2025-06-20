//using UnityEngine;

//public class BlueBlade1 : AbstractMeleeWeapon
//{
//    protected float bladeDamageMultiplier = 1f;
//    protected float bladeAttackDelayMultiplier = 4f; // Giảm thời gian delay để đánh nhanh hơn

//    protected override void Start()
//    {
//        base.Start();
//        UpdateRageModeStats_MeleeWeapons();
//    }

//    protected override void Update()
//    {
//        base.Update();

//        bool isRage = gameManager.IsRageActive();
//        if (isRage != lastRageState)
//        {
//            UpdateRageModeStats_MeleeWeapons();
//            lastRageState = isRage;
//        }

//        // Cập nhật giá trị damage và attackDelay mỗi frame để đảm bảo thay đổi có hiệu lực
//        attackDelay = isRage ?
//                      (baseAttackDelay * bladeAttackDelayMultiplier) / player.GetRageFireRateDivider() :
//                      baseAttackDelay * bladeAttackDelayMultiplier;

//        damage = isRage ?
//                 baseDamage * bladeDamageMultiplier * player.GetRageDamageMultiplier() :
//                 baseDamage * bladeDamageMultiplier;
//    }

//    protected void UpdateRageModeStats_MeleeWeapons()
//    {
//        if (gameManager.IsRageActive()) // Nếu bật Rage Mode
//        {
//            attackDelay = (baseAttackDelay * bladeAttackDelayMultiplier) / player.GetRageFireRateDivider();
//            damage = baseDamage * bladeDamageMultiplier * player.GetRageDamageMultiplier();
//        }
//        else // Nếu tắt Rage Mode
//        {
//            attackDelay = baseAttackDelay * bladeAttackDelayMultiplier;
//            damage = baseDamage * bladeDamageMultiplier;
//        }

//        Debug.Log($"Rage Mode: {gameManager.IsRageActive()}, Damage: {damage}, Attack Delay: {attackDelay}");
//    }
//}

using UnityEngine;

public class BlueBlade1 : AbstractMeleeWeapon
{
    [SerializeField] protected float bladeDamageMultiplier = 1f;
    [SerializeField] protected float bladeAttackDelayMultiplier = 4f;
    [SerializeField] protected float bladeKnockbackForce = 1f;
    [SerializeField] protected float bladeStunDuration = 3f;

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

        attackDelay = baseAttackDelay * bladeAttackDelayMultiplier / fireRateDivider;
        damage = baseDamage * bladeDamageMultiplier * rageMultiplier;
        //attackingDuration = baseAttackingDuration / fireRateDivider;
        meleeWeaponKnockbackForce = baseMeleeWeaponKnockbackForce * bladeKnockbackForce * rageKnockbackForceMultiplier;
        meleeWeaponStunDuration = baseMeleeWeaponStunDuration * bladeStunDuration * rageStunDurationMultiplier;

        // Debug thông tin Rage Mode
        //Debug.Log($"Rage Mode: {isRageActive}, Damage: {damage}, Attack Delay: {attackDelay}");
        //Debug.Log($"Base Damage: {baseDamage}, Attack Delay: {baseAttackDelay}, Rage Fire Rate Divider: {player.GetRageFireRateDivider()}");

    }
}
