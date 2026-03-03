using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AimArmor", menuName = "Scriptables/CharacterStats/Attacks/AimArmor")]
public class AimArmor : Attack
{
    public override bool ExecuteAttack(Node tile, EntityBehaviour target, Weapon weaponUsed, LayerMask userLayer)
    {
        if (target.gameObject.layer != userLayer.value)
        {
            int armorRemoved = target.CurrentWeapon.weaponDef;
            target.OnModifyArmor(-armorRemoved);
            return true;
        }
        return false;
    }
}
