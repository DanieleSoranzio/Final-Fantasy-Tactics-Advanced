using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Color = UnityEngine.Color;

public class EntityBehaviour : MonoBehaviour, IEntityStatsModifier
{
    #region Data

    Vector3 direction;
    bool hasMoved;
    bool hasAttacked;
    Node currentTile;
    TextMeshPro damageText;
    Dissolve dissolve;

    [Header("Movement Settings")]
    [SerializeField] float movementSpeed;

    Color highlightColor;

    [Header("Data")]
    [SerializeField] Entity_Data character_Data;
    Stats characterStats;
    ActionType action;

    

    //Weapon
    Weapon_Data weaponData;
    Weapon weapon;
    Attack currentAttack;

    //Getter
    public float MovementSpeed => movementSpeed;
    public float Jump => characterStats.jump / 2;
    public int MovementRange => characterStats.movementRange;
    public Color HighlightColor => highlightColor;
    public List<Attack> WeaponAttackList => weapon.weaponAttackList;
    public Stats CharacterStats => characterStats;
    public Weapon CurrentWeapon => weapon;
    public bool HasMoved
    {
        get { return hasMoved; }
        set { hasMoved = value; }
    }
    public bool HasAttacked
    {
        get { return hasAttacked; }
        set { hasAttacked = value; }
    }
    public Action<bool,bool> endAction;
    public Action endTurn;
    public Action startedMove;
    public Action startedAttack;
    #endregion

    #region Mono

    private void Awake()
    {
        dissolve = GetComponent<Dissolve>();
        damageText = GetComponentInChildren<TextMeshPro>();
        damageText.enabled = false;
        InitializeStats();
    }
    private void Start()
    {
        action = ActionType.NONE;
        gameObject.SetActive(true);
    }
    #endregion

    #region Methods
    private void InitializeStats()
    {
        characterStats = new Stats(character_Data);
        Debug.Log($" name: {characterStats.entityName} jump: {characterStats.jump} maxHP: {characterStats.maxHp} maxMP {characterStats.maxMp} speed {characterStats.speed} movementRange {characterStats.movementRange}");
        InitializeWeapon();
    }

    private void InitializeWeapon()
    {
        weaponData = character_Data.Weapon;
        weapon = new Weapon(weaponData);
        Debug.Log($"weapon name: {weapon.weaponName} weapon Atk: {weapon.weaponAtk} weapon Def {weapon.weaponDef} weapon accuracy: {weapon.weaponAccuracy} weapon Attack List {weapon.weaponAttackList}");
    }
    private void EndAction()
    {
        action = ActionType.NONE;
        if (hasMoved && hasAttacked)
        {
            EndTurn();
            return;
        }
        endAction?.Invoke(hasMoved,hasAttacked);
    }
    private void EndTurn()
    {
        endTurn?.Invoke();
    }
    public void TileSelected(Node tile,EntityBehaviour enemyOnTile)
    {
        if (action == ActionType.MOVE && enemyOnTile==null && !hasMoved)
        {
            hasMoved = true;
            Move(tile);
        }
        else if (action == ActionType.ATTACK && enemyOnTile != null && !hasAttacked)
        {
            hasAttacked=true;
            Attack(tile,enemyOnTile);
        }
    }

    public void MoveSelected()
    {
        if(!hasMoved)
        {
            action = ActionType.MOVE;
            NodeManager.Instance.GetArea(this, characterStats.movementRange, characterStats.jump, true,false);
        }
    }
    public void AttackSelected(int attackNumber) 
    {
        action=ActionType.ATTACK;
        currentAttack = weapon.weaponAttackList[attackNumber];
        if(currentAttack.Cost<= characterStats.currentMp)
        {
            NodeManager.Instance.GetArea(this, currentAttack.RangeATK, currentAttack.HeightDiff, false, true);
        }
    }
    private void Move(Node endTile)
    {
        Debug.Log($"Moving to {endTile}");
        startedMove?.Invoke();
        NodeManager.Instance.MoveTo(this, endTile);
    }
    public void EndMove()
    {
        EndAction();
    }

    private void Attack(Node tile, EntityBehaviour enemy)
    {
        //attacca con l'attacco selezionato
        if (currentAttack.ExecuteAttack(tile, enemy, weapon,gameObject.layer))
        {
            startedAttack?.Invoke();
            characterStats.currentMp -= currentAttack.Cost;
            Debug.Log("Attacking on tile: " + enemy.character_Data.Name);
            EndAttack();
        }
    }
    private void EndAttack()
    {
        EndAction();
    }
    public Node GetCurrentTile()
    {
        if (Physics.Raycast(this.transform.position + Vector3.up, Vector3.down, out RaycastHit hit, float.MaxValue))
        {
            currentTile = hit.collider.gameObject.GetComponent<Node>();
        }
        return currentTile;
    }
    public void SetDirection(Vector3 dir)
    {
        if (direction != dir)
        {
            direction = dir;
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }

    public void GetColor(Color highlightColor)
    {
        this.highlightColor= highlightColor;
    }

    public void DamageReceiver(Weapon weaponUsed,Attack attackUsed)
    {
        int calculatedDamage = DamageFormula.FormulaReturnDamage(weaponUsed,attackUsed,weapon,characterStats);
        OnTakeDamage(calculatedDamage);
    }

    public void OnTakeDamage(int damage = 0)
    {
        
        characterStats.currentHp -= damage;
        Debug.Log(characterStats.currentHp);
        characterStats.currentHp = Math.Clamp(characterStats.currentHp,0,characterStats.maxHp);
        Debug.Log(characterStats.currentHp);
        if(characterStats.currentHp == 0)
        {
            StartCoroutine(Die());
        }
        StartCoroutine(ShowText(damage));
    }
    private IEnumerator ShowText(int damage=0, Color? color = null)
    {
        damageText.enabled = true;
        damageText.color = color ?? Color.black;
        damageText.transform.LookAt(Camera.main.transform);
        damageText.transform.Rotate(0,180,0);
        float elapsedTime = 0f;
        float duration = 1.5f;
        Vector3 startingPositiontext = damageText.transform.localPosition;
        Vector3 nextPosition = new Vector3(startingPositiontext.x, 2, startingPositiontext.z);
        if (damage == 0)
            damageText.text = "MISS";
        else
            damageText.text = damage.ToString();
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);
            damageText.transform.localPosition = Vector3.Lerp(startingPositiontext,nextPosition,t);

            yield return null;
        }
        damageText.enabled = false;
        damageText.transform.localPosition = startingPositiontext;
    }
    
    public IEnumerator Die()
    {
        GameManager.Instance.CharacterDied(this);
        yield return dissolve.StartCoroutine(dissolve.ApplyDissolveMaterial());
        Debug.Log(this);
        gameObject.SetActive(false);
    }

    public void OnHeal(int heal)
    {
        characterStats.currentHp += heal;
        characterStats.currentMp = Math.Clamp(characterStats.currentHp,0,characterStats.maxHp);
        StartCoroutine(ShowText(heal,Color.green));
    }
    public void OnModifySpeed(int amount)
    {
        characterStats.speed += amount;
        Color color = amount > 0 ? Color.yellow : Color.red;
        StartCoroutine(ShowText(amount,color));  
    }
    public void OnModifyMagicRes(int amount)
    {
        characterStats.mageRes += amount;
        Color color = amount > 0 ? Color.magenta : Color.red;
        StartCoroutine(ShowText(amount, color));
    }
    public void OnModifyArmor(int amount)
    {
        weapon.weaponDef += amount;
        Color color = amount > 0 ? Color.magenta : Color.red;
        StartCoroutine(ShowText(amount, color));
    }
    public void OnModifyEvade(int amount)
    {
        characterStats.evade += amount;
        Color color = amount > 0 ? Color.yellow : Color.red;
        StartCoroutine(ShowText(amount, color));
    }
    #endregion
}
[System.Serializable]
public struct Weapon
{
    public Sprite icon;
    public string weaponName;
    public int weaponAtk;
    public int weaponDef;
    public int weaponAccuracy;
    public List<Attack> weaponAttackList;
    public Weapon(Weapon_Data weapon)
    {
        icon = weapon.Icon;
        weaponName = weapon.WeaponName;
        weaponAtk = weapon.WeaponAttack;
        weaponAccuracy = weapon.WeaponAccuracy;
        weaponDef = weapon.WeaponDef;
        weaponAttackList = weapon.AttackList;
    }
}
[System.Serializable]
public struct Stats
{
    public string entityName;
    public float jump;
    public int currentHp;
    public int currentMp;
    public int maxHp;
    public int maxMp;
    public int speed;
    public int movementRange;
    public int evade;
    public int magePow;
    public int mageRes;
    public Stats(Entity_Data entity)
    {
        entityName = entity.Name;
        jump = entity.Jump;
        currentHp = entity.MaxHp;
        currentMp = entity.MaxMp;
        maxHp = entity.MaxHp;
        maxMp = entity.MaxMp;
        speed = entity.Speed;
        movementRange = entity.MovementRange;
        evade = entity.Evade;
        magePow = entity.MagePower;
        mageRes = entity.MageResistance;
    }
}