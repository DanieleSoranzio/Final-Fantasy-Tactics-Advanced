using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    #region UI 

    [Header("BattleUI"),Space(10)]

    [SerializeField] GameObject playerUiPanel;
    [SerializeField] GameObject DecisionPanel;
    [SerializeField] GameObject movePanel;
    [SerializeField] GameObject attacksPanel;
    [SerializeField] GameObject statusPanel;
    [SerializeField] Button moveButton;
    [SerializeField] Button attackButton;
    [SerializeField] List<TextMeshProUGUI> attacksText = new List<TextMeshProUGUI>();
    [SerializeField] GameObject winPanel;
    [SerializeField] GameObject lostPanel;

    [Header("StatusUI")]

    [Header("GeneralStats"), Space(10)]

    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI moveNumberText;
    [SerializeField] TextMeshProUGUI jumpNumberText;
    [SerializeField] TextMeshProUGUI evadeNumberText;
    [SerializeField] TextMeshProUGUI weaponAtkText;
    [SerializeField] TextMeshProUGUI weaponDefText;
    [SerializeField] TextMeshProUGUI magicPowText;
    [SerializeField] TextMeshProUGUI magicResText;
    [SerializeField] TextMeshProUGUI speedNumberText;

    [Header("Hp and Mp"),Space(10)]

    [SerializeField] TextMeshProUGUI currentAndMaxHpText;
    [SerializeField] TextMeshProUGUI currentAndMaxMpText;
    [SerializeField] Slider hpSlider;
    [SerializeField] Slider mpSlider;

    [Header("Equip"), Space(10)]

    [SerializeField] TextMeshProUGUI weaponName;
    [SerializeField] Image weaponSprite;
    [SerializeField] List<TextMeshProUGUI> attacksNameText = new List<TextMeshProUGUI>();

    [Header("AttackDescription"), Space(10)]

    [SerializeField] TextMeshProUGUI descriptionText;
    [SerializeField] TextMeshProUGUI costNumberText;
    [SerializeField] TextMeshProUGUI rangeNumberText;
    [SerializeField] TextMeshProUGUI damageNumberText;
    [SerializeField] TextMeshProUGUI typeText;
    [SerializeField] TextMeshProUGUI heightText;

    [Header("Turn List"), Space(10)]

    [SerializeField] TextMeshProUGUI textTurnCount;
    [SerializeField] GameObject turnText;
    [SerializeField] GameObject parent;

    int turnCount;
    Stats currentCharacterStats;
    Weapon currentCharacterWeapon;
    #endregion

    #region Mono
    private void OnEnable()
    {
        EventManager.updateAttackListUI += UpdateAttacksPanel;
        EventManager.OnStatus += OnOpenStatus;
        EventManager.OnStartAction += OnStartAction;
        EventManager.startedMoving += moveActionUsed;
        EventManager.startedAttacking += attackActionUsed;
        EventManager.OnEndTurn += OnEndTurn;
        EventManager.GameOver += GameOver;
        EventManager.GameWon += GameWon;
        EventManager.newTurnList += updateTurnListUI;
    }

   

    private void OnDisable()
    {
        EventManager.updateAttackListUI -= UpdateAttacksPanel;
        EventManager.OnStartAction -= OnStartAction;
        EventManager.startedMoving -= moveActionUsed;
        EventManager.startedAttacking -= attackActionUsed;
        EventManager.OnEndTurn -= OnEndTurn;
        EventManager.OnStatus -= OnOpenStatus;
        EventManager.GameOver -= GameOver;
        EventManager.GameWon -= GameWon;
        EventManager.newTurnList -= updateTurnListUI;
    }



    private void Start()
    {
        statusPanel.SetActive(false);
        playerUiPanel.SetActive(false);
    }
    #endregion

    #region Methods
    private void moveActionUsed()
    {
        ActionStarted();
        SetMoveButtonInteraction(false);
    }
    private void attackActionUsed()
    {
        ActionStarted();
        SetAttackButtonInteraction(false);
    }
    private void ActionStarted()
    {
        playerUiPanel.SetActive(false);
    }
    private void SetMoveButtonInteraction(bool interaction)
    {
        moveButton.interactable = interaction;
    }
    private void SetAttackButtonInteraction(bool interaction)
    {
        attackButton.interactable= interaction;
    }
    public void MoveButton() 
    {
        EventManager.moveSelected?.Invoke();
        DecisionPanel.SetActive(false);
        movePanel.SetActive(true);
    }

    public void ActionButton()
    {
        DecisionPanel.SetActive(false);
        attacksPanel.SetActive(true);
        EventManager.attackSelected?.Invoke();
    }
    public void AttackSelected(int attack)
    {
        EventManager.attackChosen?.Invoke(attack);
    }

    public void StatusButton()
    {
        statusPanel.SetActive(true);
        EventManager.statusSelected?.Invoke();
    }
    private void OnOpenStatus(Stats stats, Weapon weapon)
    {
        statusPanel.SetActive(true);

        //Save instance       
        currentCharacterStats = stats;
        currentCharacterWeapon = weapon;

        //General Stats

        SetText(nameText, stats.entityName);
        SetText(moveNumberText, stats.movementRange);
        SetText(jumpNumberText, stats.jump);
        SetText(evadeNumberText, stats.evade);
        SetText(weaponAtkText, weapon.weaponAtk);
        SetText(weaponDefText, weapon.weaponDef);
        SetText(magicPowText, stats.magePow);
        SetText(magicResText, stats.mageRes);
        SetText(speedNumberText, stats.speed);

        //Hp and Mp

        SetText(currentAndMaxHpText, $"{stats.currentHp} / {stats.maxHp}");
        SetText(currentAndMaxMpText, $"{stats.currentMp} / {stats.maxMp}");
        hpSlider.maxValue = stats.maxHp;
        hpSlider.value = stats.currentHp;
        mpSlider.maxValue = stats.maxMp;
        mpSlider.value = stats.currentMp;

        //Weapon

        weaponSprite.sprite = weapon.icon;
        SetText(weaponName, weapon.weaponName);

        //Attacks

        for(int i=0;i<attacksNameText.Count;i++)
        {
            SetText(attacksNameText[i], weapon.weaponAttackList[i].AttackName);
        }

    }
    public void OnDescription(int number)
    {
        SetText(descriptionText, currentCharacterWeapon.weaponAttackList[number].Description);
        SetText(costNumberText, currentCharacterWeapon.weaponAttackList[number].Cost);
        SetText(rangeNumberText, currentCharacterWeapon.weaponAttackList[number].RangeATK);
        SetText(damageNumberText, currentCharacterWeapon.weaponAttackList[number].AttackDamage);
        SetText(typeText, currentCharacterWeapon.weaponAttackList[number].DamageType);

        if(currentCharacterWeapon.weaponAttackList[number].HeightDiff >= int.MaxValue)
        {
            SetText(heightText, "Inf");
        }
        else
            SetText(heightText, currentCharacterWeapon.weaponAttackList[number].HeightDiff);
    }
    private void updateTurnListUI(List<EntityBehaviour> list)
    {
        turnCount++;
        textTurnCount.text= turnCount.ToString();   
        int childCount = parent.transform.childCount;
        for (int i = childCount - 1; i >= 0; i--)
        {
            Destroy(parent.transform.GetChild(i).gameObject);
        }
        foreach (var character in list)
        {
            var newText = Instantiate(turnText, parent.transform);
            newText.GetComponent<TextMeshProUGUI>().text = character.CharacterStats.entityName;
        }
    }
    private void SetText(TextMeshProUGUI text,object obj)
    {
        text.text =obj.ToString();
    }

    public void WaitButton()
    {
        EventManager.waitSelected?.Invoke();
    }

    public void BackSelected()
    {
        DecisionPanel.SetActive(true);
        movePanel.SetActive(false);
        attacksPanel.SetActive(false);
        statusPanel.SetActive(false);
        EventManager.backSelected?.Invoke();
    }

    /// <summary>
    /// Called on event when new player turn start
    /// </summary>
    private void OnStartAction(bool interactionMove,bool interactionAttack)
    {
        playerUiPanel.SetActive(true);
        DecisionPanel.SetActive(true);
        movePanel.SetActive(false);
        attacksPanel.SetActive(false);
        statusPanel.SetActive(false);
        SetMoveButtonInteraction(!interactionMove);
        SetAttackButtonInteraction(!interactionAttack);
    }

    private void OnEndTurn()
    {
        playerUiPanel.SetActive(false);
        DecisionPanel.SetActive(true);
        movePanel.SetActive(false);
        attacksPanel.SetActive(false);
        statusPanel.SetActive(false);
        SetMoveButtonInteraction(true);
        SetAttackButtonInteraction(true);
    }
    private void UpdateAttacksPanel(List<Attack> attackList)
    {
        for (int i = 0; i < attackList.Count; i++)
        {
            attacksText[i].text = attackList[i].AttackName;
        }
    }
    private void GameOver()
    {
        lostPanel.SetActive(true);
    }

    private void GameWon()
    {
        winPanel.SetActive(true);
    }

    #endregion
}
