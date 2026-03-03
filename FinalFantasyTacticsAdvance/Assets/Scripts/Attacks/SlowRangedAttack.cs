using UnityEngine;

[CreateAssetMenu(fileName = "SlowRangedAttack", menuName = "Scriptables/CharacterStats/Attacks/SlowRangedAttack")]

public class SlowRangedAttack : Attack
{
    [SerializeField]int speedRemoved;
    public override bool ExecuteAttack(Node tile, EntityBehaviour target, Weapon weaponUsed, LayerMask userLayer)
    {
        if (target.gameObject.layer != userLayer.value)
        {
            target.OnModifySpeed(-speedRemoved);
            target.DamageReceiver(weaponUsed,this);
            return true;
        }
        return false;
    }
}
