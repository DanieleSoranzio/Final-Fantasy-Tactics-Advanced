using UnityEngine;

public static class DamageFormula
{

    #region Methods
    public static int FormulaReturnDamage(Weapon weaponUsed,Attack attackUsed,Weapon defenderWeapon,Stats defender)
    {
        int damage = 0;

        if (EvadeFormula(weaponUsed,defender))
        {
            return damage;
        }
        if(attackUsed.DamageType==DamageType.PHYSICAL)
        {
            damage = CalculatePhysicalAttack(weaponUsed,attackUsed,defenderWeapon);
        }
        else
        {
            damage = CalculateMagicAttack(weaponUsed,attackUsed,defenderWeapon);
        }
       
        return damage;
    }
    public static bool EvadeFormula(Weapon wepaonUsed,Stats defender)
    {
        bool isEvaded=true;
        int chance;
        int RandomNumber = Random.Range(1, 101);
        chance = wepaonUsed.weaponAccuracy - defender.evade;
        chance = Mathf.Clamp(chance, 0, chance);
        if(RandomNumber<=chance)
        {
            isEvaded = false;
        }
        return isEvaded;
    }
    private static int CalculatePhysicalAttack(Weapon weaponUsed,Attack attackUsed,Weapon defenderWeapon)
    {
        int damage = 0;
        damage = ((attackUsed.AttackDamage - defenderWeapon.weaponDef) / 2) * (weaponUsed.weaponAtk);
        damage = Mathf.Clamp(damage, 1, int.MaxValue);
        return damage;
    }
    private static int CalculateMagicAttack(Weapon weaponUsed, Attack attackUsed, Weapon defenderWeapon)
    {
        int damage = 0;
        damage = weaponUsed.weaponAtk * (attackUsed.AttackDamage - defenderWeapon.weaponDef);
        damage = Mathf.Clamp(damage, 1, int.MaxValue);
        return damage;
    }
    #endregion
}
