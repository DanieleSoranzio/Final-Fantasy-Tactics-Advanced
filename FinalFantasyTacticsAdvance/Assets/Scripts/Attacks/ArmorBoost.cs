using UnityEngine;

[CreateAssetMenu(fileName = "ArmorBoost", menuName = "Scriptables/CharacterStats/Attacks/ArmorBoost")]
public class ArmorBoost : Attack
{
    int armorGiven=2;
    public override bool ExecuteAttack(Node tile, EntityBehaviour target, Weapon weaponUsed, LayerMask userLayer)
    {
        if(target.gameObject.layer==userLayer.value)
        {
            target.OnModifyArmor(armorGiven);
            return true;
        }
        return false;
    }
}
