using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EnemyController : MonoBehaviour, ICharacterController
{
    #region Data
    EntityBehaviour activePawn;
    [SerializeField] Color HighlightColorTile;
    int enemymaskInt=7;
    public EntityBehaviour ActivePawn => activePawn;
    EntityBehaviour target;
    bool hasAttacked;
    bool hasMoved;
    #endregion


    #region Mono
    private void Start()
    {
        GameManager.Instance.RegisterAIController(this);
    }
    #endregion


    #region Methods
    IEnumerator StartAction()
    {
        yield return new WaitForEndOfFrame();
        Debug.Log($"Active pawn has attacked? {hasAttacked}");
        if(!hasAttacked)
        {
            StartCoroutine(SelectRandomAttack());
        }
        else
        {
            Wait();
        }
       
    }
    private IEnumerator SelectRandomAttack()
    {
        int attackNumber = Random.Range(0,activePawn.WeaponAttackList.Count);
        activePawn.AttackSelected(attackNumber);
        Node tileSelected = CheckIfCellsAreOccupied(out target);
        yield return new WaitForSeconds(0.7f);
        if (tileSelected != null && target!=null)
        {
            Debug.Log("Attacking!");
            hasAttacked = true;
            activePawn.TileSelected(tileSelected,target);
            yield return null;
        }
        else if(!hasMoved)
        {
            Debug.Log("Enemy NOT found in range!");
            StartCoroutine(MoveSelected());
            yield return null;
        }
        else
        {
            Wait();
            yield return null;
        }
    }
    private IEnumerator MoveSelected()
    {
        activePawn.MoveSelected();
        List<Node> enemyTiles = NodeManager.Instance.GiveEnemyPosition(enemymaskInt);
        Node closestEnemyTile = FindClosestTile(enemyTiles,activePawn.GetCurrentTile());
        Node endTile = FindClosestTile(NodeManager.Instance.HighlightedTiles, closestEnemyTile);
        hasMoved = true;
        yield return new WaitForSeconds(0.5f);
        activePawn.TileSelected(endTile,null);
    }

    private void Wait()
    {
        EndTurn();
    }
    private Node CheckIfCellsAreOccupied(out EntityBehaviour target)
    {
        target = null;
        for(int i = 0;i< NodeManager.Instance.HighlightedTiles.Count;i++)
        {
            if(NodeManager.Instance.HighlightedTiles[i].CheckIfOccupied() && NodeManager.Instance.HighlightedTiles[i].LayerOfTheOccupieing==enemymaskInt)
            {
                target = NodeManager.Instance.HighlightedTiles[i].EntityOnTile();
                if (target != null)
                {
                    Debug.Log($"This cell is occupied by enemy {NodeManager.Instance.HighlightedTiles[i].name} and {target.name}");
                }
                return NodeManager.Instance.HighlightedTiles[i];
            }
            else
            {
                Debug.Log($"This cell is NOT occupied by enemy {NodeManager.Instance.HighlightedTiles[i].name}");
                continue;
            }
        }
        return null;
    }
    private void EndAction(bool hasMoved,bool hasAttacked)
    {
        NodeManager.Instance.ClearHighlighedTiles();
        if(hasAttacked)
        {
            Wait();
            return;
        }
        StartCoroutine(StartAction());
    }
    private void EndTurn()
    {
        NodeManager.Instance.ClearHighlighedTiles();
        hasAttacked=false;
        hasMoved = false;
        activePawn.HasMoved = false;
        activePawn.HasAttacked = false;
        GameManager.Instance.StartTurn();
    }
    Node FindClosestTile(List<Node> tiles, Node finalTile)
    {
        if (tiles == null || tiles.Count == 0 || finalTile == null)
        {
            return null;
        }

        Node closest = tiles[0];
        float minDistance = CalculateDistance(tiles[0].transform, finalTile.transform);

        foreach (Node tile in tiles)
        {
            float distance = CalculateDistance(tile.transform, finalTile.transform);
            if (distance < minDistance)
            {
                minDistance = distance;
                closest = tile;
            }
        }

        return closest;
    }

    float CalculateDistance(Transform tile1, Transform tile2)
    {
        // Calculate the distance considering only the X and Z coordinates
        Vector3 pos1 = tile1.position;
        Vector3 pos2 = tile2.position;
        float distance = Mathf.Sqrt(Mathf.Pow(pos2.x - pos1.x, 2) + Mathf.Pow(pos2.z - pos1.z, 2));
        return distance;
    }
    #endregion


    public void Select(EntityBehaviour character)
    {
        activePawn = character;
        activePawn.GetColor(HighlightColorTile);
        activePawn.endAction += EndAction;
        activePawn.endTurn += EndTurn;  
        StartCoroutine(StartAction());
    }

    public void Unselect()
    {
        activePawn.endAction -= EndAction;
        activePawn.endTurn -= EndTurn;
        activePawn = null;
    }
}
