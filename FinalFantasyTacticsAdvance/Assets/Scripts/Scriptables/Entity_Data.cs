using System;
using UnityEditor;
using UnityEngine;


[CreateAssetMenu(fileName = "Character", menuName = "Scriptables/CharacterStats/Character")]
public class Entity_Data : ScriptableObject
{
    #region Data
    [Header("Stats")]
    [SerializeField] string entityName;
    [SerializeField] float jump;
    [SerializeField] int evade;
    [SerializeField] int maxHp;
    [SerializeField] int maxMp;
    [Tooltip("Speed of the character will change the turn sequence")]
    [SerializeField] int speedOfTurn;
    [SerializeField] int movementRange;
    [SerializeField] int magePower;
    [SerializeField] int mageResistance;
    [SerializeField] Weapon_Data weapon;
 
    //Getter
    public string Name => entityName;
    public float Jump => jump;
    public int Evade => evade;  
    public int MaxHp => maxHp;
    public int MaxMp => maxMp;
    public int Speed => speedOfTurn;
    public int MovementRange => movementRange;
    public int MagePower => magePower;
    public int MageResistance => mageResistance;
    public Weapon_Data Weapon => weapon;
    #endregion


    #region Mono

    #endregion


    #region Methods

    #endregion
}