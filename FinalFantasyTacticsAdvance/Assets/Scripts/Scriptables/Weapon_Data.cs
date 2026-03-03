using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Weapon", menuName = "Scriptables/CharacterStats/Weapon")]
public class Weapon_Data : ScriptableObject
{
    [SerializeField] Sprite iconWeapon;
    [SerializeField] string weaponName;
    [SerializeField] int weaponAtk;
    [SerializeField] int weaponDef;
    [SerializeField] int weaponAccuracy;
    [SerializeField] List<Attack> attackList=new List<Attack>();

    //Getter
    public Sprite Icon => iconWeapon;
    public string WeaponName => weaponName;
    public int WeaponAttack => weaponAtk;
    public int WeaponDef => weaponDef;
    public int WeaponAccuracy => weaponAccuracy;  
    public List<Attack> AttackList => attackList;
}
