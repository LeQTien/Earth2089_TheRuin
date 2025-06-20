//using UnityEngine;

//public class RedAxe : AbstractMeleeWeapon
//{
//    protected float AxeDamageMultiplier = 2f;
//    protected float AxeAttackDelayMultiplier = 8f; // Giảm thời gian delay để đánh nhanh hơn

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
//        // Kiểm tra giá trị damage có thay đổi đúng không
//        Debug.Log($"Update Frame - Rage Mode: {isRage}, Damage: {damage}, Attack Delay: {attackDelay}");

//        // Cập nhật giá trị damage và attackDelay mỗi frame để đảm bảo thay đổi có hiệu lực
//        attackDelay = isRage ?
//                      (baseAttackDelay * AxeAttackDelayMultiplier) / player.GetRageFireRateDivider() :
//                      baseAttackDelay * AxeAttackDelayMultiplier;

//        damage = isRage ?
//                 baseDamage * AxeDamageMultiplier * player.GetRageDamageMultiplier() :
//                 baseDamage * AxeDamageMultiplier;
//    }

//    protected void UpdateRageModeStats_MeleeWeapons()
//    {
//        if (gameManager.IsRageActive()) // Nếu bật Rage Mode
//        {
//            attackDelay = (baseAttackDelay * AxeAttackDelayMultiplier) / player.GetRageFireRateDivider();
//            damage = baseDamage * AxeDamageMultiplier * player.GetRageDamageMultiplier();
//        }
//        else // Nếu tắt Rage Mode
//        {
//            attackDelay = baseAttackDelay * AxeAttackDelayMultiplier;
//            damage = baseDamage * AxeDamageMultiplier;
//        }

//        Debug.Log($"Rage Mode: {gameManager.IsRageActive()}, Damage: {damage}, Attack Delay: {attackDelay}");
//    }
//}


using UnityEngine;

public class RedAxe : AbstractMeleeWeapon
{
    [SerializeField] protected float axeDamageMultiplier = 2f;
    [SerializeField] protected float axeAttackDelayMultiplier = 8f;
    [SerializeField] protected float axeKnockbackForce = 1.5f;
    [SerializeField] protected float axeStunDuration = 5f;

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

        attackDelay = baseAttackDelay * axeAttackDelayMultiplier / fireRateDivider;
        damage = baseDamage * axeDamageMultiplier * rageMultiplier;
        //attackingDuration = baseAttackingDuration / fireRateDivider;
        meleeWeaponKnockbackForce = baseMeleeWeaponKnockbackForce * axeKnockbackForce * rageKnockbackForceMultiplier;
        meleeWeaponStunDuration = baseMeleeWeaponStunDuration * axeStunDuration * rageStunDurationMultiplier;

        // Debug thông tin Rage Mode
        //Debug.Log($"Rage Mode: {isRageActive}, Damage: {damage}, Attack Delay: {attackDelay}");
        //Debug.Log($"Base Damage: {baseDamage}, Attack Delay: {baseAttackDelay}, Rage Fire Rate Divider: {player.GetRageFireRateDivider()}");

    }
}
