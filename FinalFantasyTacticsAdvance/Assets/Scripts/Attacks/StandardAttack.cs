using UnityEngine;

[CreateAssetMenu(fileName = "StandardAttack", menuName = "Scriptables/CharacterStats/Attacks/StandardAttack")]
public class StandardAttack : Attack
{
    
    public override bool ExecuteAttack(Node tile, EntityBehaviour target, Weapon weaponUsed, LayerMask userLayer)
    {
        Debug.Log($"target layer {target.gameObject.layer}, my layer is {userLayer.value}");
        if (target.gameObject.layer != userLayer.value)
        {
            target.DamageReceiver(weaponUsed, this);
            return true;
        }
        return false;
    }
}
