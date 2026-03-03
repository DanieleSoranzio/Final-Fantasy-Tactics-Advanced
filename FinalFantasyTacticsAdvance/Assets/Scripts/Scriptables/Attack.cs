using UnityEngine;


public abstract class Attack : ScriptableObject
{
    [SerializeField,TextArea] string descriptionAttack;
    [SerializeField] string attackName;
    [SerializeField] int rangeOfAttack;
    [SerializeField] int attackDamage;
    [SerializeField] int cost;
    [SerializeField] int heightDiff=int.MaxValue;
    [SerializeField] DamageType damageType;
    //Getter
    public string Description => descriptionAttack;
    public string AttackName => attackName;
    public int RangeATK => rangeOfAttack;
    public int AttackDamage => attackDamage;
    public int Cost => cost;    
    public int HeightDiff => heightDiff;
    public DamageType DamageType => damageType;
    public abstract bool ExecuteAttack(Node tile, EntityBehaviour target,Weapon weaponUsed,LayerMask userLayer);
}
