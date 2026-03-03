using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerController : MonoBehaviour, ICharacterController
{
    #region Data
    Controls controls;
    EntityBehaviour activePawn;
    Camera cam;


    [Header("Settings")]
    [SerializeField] LayerMask tilesMask;
    [SerializeField] Color HighlightColorTile;
    [SerializeField] LayerMask enemyMask;

    public EntityBehaviour ActivePawn => activePawn;
    #endregion


    #region Mono
    private void Awake()
    {
        GameManager.Instance.RegisterPlayerController(this);

        controls = new Controls();

        cam = Camera.main;
    }

    private void OnEnable()
    {
        if (!controls.Player.enabled)
        {
            controls.Player.Enable();
            controls.Player.Select.performed += OnSelectPerformed;
        }

        EventManager.moveSelected += MoveButton;
        EventManager.attackSelected += AttackButton;
        EventManager.attackChosen += AttackSelected;
        EventManager.statusSelected += StatusButton;
        EventManager.waitSelected += WaitButton;
        EventManager.backSelected += BackButton;

    }


    private void OnDisable()
    {
        if (controls.Player.enabled)
        {
            controls.Player.Disable();
            controls.Player.Select.performed -= OnSelectPerformed;
        }

        EventManager.moveSelected -= MoveButton;
        EventManager.attackSelected -= AttackButton;
        EventManager.attackChosen -= AttackSelected;
        EventManager.statusSelected -= StatusButton;
        EventManager.waitSelected -= WaitButton;
        EventManager.backSelected -= BackButton;

    }
    #endregion

    #region Methods
    /// <summary>
    /// called by event when the character controlled finished one action but not the turn
    /// </summary>
    private void EndAction(bool hasmoved, bool hasAttacked)
    {
        NodeManager.Instance.ClearHighlighedTiles();
        EventManager.OnStartAction?.Invoke(hasmoved, hasAttacked);
    }
    private void EndTurn()
    {
        NodeManager.Instance.ClearHighlighedTiles();
        activePawn.HasAttacked = false;
        activePawn.HasMoved = false;
        EventManager.OnEndTurn?.Invoke();
        GameManager.Instance.StartTurn();
    }
    /// <summary>
    /// Called on event when move button is clicked
    /// </summary>
    private void MoveButton()
    {
        activePawn.MoveSelected();
    }

    /// <summary>
    /// Called on event when attack button is clicked
    /// </summary>
    private void AttackButton()
    {
        EventManager.updateAttackListUI?.Invoke(activePawn.WeaponAttackList);
    }

    /// <summary>
    /// Called on event when an attack is selected
    /// </summary>
    /// <param name="number"></param>
    private void AttackSelected(int number)
    {
        activePawn.AttackSelected(number);
    }

    /// <summary>
    /// Called on event when status button is clicked
    /// </summary>
    private void StatusButton()
    {
        EventManager.OnStatus?.Invoke(activePawn.CharacterStats, activePawn.CurrentWeapon);
    }

    /// <summary>
    /// Called on event when wait button is clicked
    /// </summary>
    private void WaitButton()
    {
        EndTurn();
    }

    /// <summary>
    /// Called on event when the back button is clicked
    /// </summary>
    private void BackButton()
    {
        NodeManager.Instance.ClearHighlighedTiles();
    }

    private void OnSelectPerformed(InputAction.CallbackContext context)
    {
        EntityBehaviour enemy = null;
        if (NodeManager.Instance.HighlightedTiles.Count == 0 || activePawn == null )
        {
            if (Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out RaycastHit enemyFound, float.MaxValue))
            {
                Debug.Log(enemyFound.collider.name);
                enemy = enemyFound.collider.GetComponentInParent<EntityBehaviour>();
                EventManager.OnStatus?.Invoke(enemy.CharacterStats, enemy.CurrentWeapon);
            }
        }
        if (activePawn == null) return;
        
        if (Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, float.MaxValue, tilesMask))
        {
            Node tile = hit.collider.GetComponent<Node>();
            if (tile.Highlighted)
            {
                if (Physics.Raycast(tile.transform.position+Vector3.down, Vector3.up, out RaycastHit enemyFound, float.MaxValue))
                {
                    enemy = enemyFound.collider.GetComponentInParent<EntityBehaviour>();
                }
                activePawn.TileSelected(tile, enemy);
            }
        }

    }
    private void characterAttacked()
    {
        EventManager.startedAttacking?.Invoke();
    }
    private void characterMoved()
    {
        EventManager.startedMoving?.Invoke();
    }
    public void Select(EntityBehaviour character)
    {
        activePawn = character;
        activePawn.GetColor(HighlightColorTile);
        activePawn.endAction += EndAction;
        activePawn.startedAttack += characterAttacked;
        activePawn.startedMove += characterMoved;
        activePawn.endTurn += EndTurn;
        EndAction(false, false);
    }

    public void Unselect()
    {
        activePawn.endAction -= EndAction;
        activePawn.startedAttack -= characterAttacked;
        activePawn.startedMove -= characterMoved;
        activePawn.endTurn -= EndTurn;
        activePawn = null;
    }

    #endregion
}
