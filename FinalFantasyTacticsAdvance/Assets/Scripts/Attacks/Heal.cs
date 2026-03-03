using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Heal", menuName = "Scriptables/CharacterStats/Attacks/Heal")]
public class Heal : Attack
{
    #region Methods
    public override bool ExecuteAttack(Node tile, EntityBehaviour target, Weapon weaponUsed, LayerMask userLayer)
    {
        Debug.Log($"target layer {target.gameObject.layer}, my layer is {userLayer}");
        if(target.gameObject.layer == userLayer.value)
        {
            if(target.CharacterStats.currentHp < target.CharacterStats.maxHp)
            {
                target.OnHeal(AttackDamage);
                return true;
            }
        }
        return false;
    }
    #endregion

}
