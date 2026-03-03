using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class NodeManager : Singleton<NodeManager>
{
    #region Variables

    NodeGraph grid;
    List<Node> highlightedTiles;
    public List<Node> HighlightedTiles=> highlightedTiles ??= new List<Node>();

    [Header("Settings")]
    [SerializeField] LayerMask tilesMask;
    [SerializeField] AnimationCurve risingTerrainYCurve;
    [SerializeField] AnimationCurve descendingTerrainYCurve;

    #endregion

    #region Mono
    protected override void Awake()
    {
        base.Awake();

        grid = new NodeGraph();
        highlightedTiles = new List<Node>();
    }
    #endregion


    #region Methods
    public List<Node> GiveEnemyPosition(int layermask)
    {
        return grid.GetEnemyTile(layermask);
    }
    public void GetArea(EntityBehaviour pawn,float range,float jump = float.MaxValue, bool filterOccupiedTiles = true,bool isAttacking = false) 
    {
        Debug.Log("Getting area");
        ClearHighlighedTiles();
        if (Physics.Raycast(pawn.transform.position + Vector3.up, Vector3.down, out RaycastHit hit, float.MaxValue, tilesMask))
        {
            Node start = hit.collider.GetComponent<Node>();
            Node[] area = grid.GetArea(start, range , jump ,filterOccupiedTiles,isAttacking);
            for (int i = 0; i < area.Length; i++)
            {
                highlightedTiles.Add(area[i]);
                highlightedTiles[i].ToggleHighlight(true, pawn.HighlightColor);
            }
        }
    } 

    public void RegisterTile(Node tile)
    {
        grid.AddTile(tile);
    }

    public void ClearHighlighedTiles()
    {
        for (int i = 0; i < highlightedTiles.Count; i++)
            highlightedTiles[i].ToggleHighlight(false);

        highlightedTiles.Clear();
    }

    public void MoveTo(EntityBehaviour character, in Node end)
    {
        if (Physics.Raycast(character.transform.position + Vector3.up, Vector3.down, out RaycastHit hit, float.MaxValue, tilesMask))
        {
            Node start = hit.collider.GetComponent<Node>();
            Node[] path = grid.GetPath(in start, in end, character.Jump);

            StartCoroutine(MoveToCR(character, path));
        }
    }

    private IEnumerator MoveToCR(EntityBehaviour character, Node[] path)
    {
        ClearHighlighedTiles();

        if (path != null)
        {
            highlightedTiles.Add(path[path.Length - 1]);
            highlightedTiles[0].ToggleHighlight(true, character.HighlightColor);

            for (int i = 0; i < path.Length - 1; i++)
                yield return StartCoroutine(TilesMovementCR(character, path[i].transform.position, path[i + 1].transform.position));
            character.EndMove();
            ClearHighlighedTiles();
        }
    }

    private IEnumerator TilesMovementCR(EntityBehaviour character, Vector3 start, Vector3 end)
    {
        float step = end.y - start.y;
        Vector3 dir = new Vector3(end.x - start.x, 0, end.z - start.z);
        character.SetDirection(dir);

        float t = 0;
        while (t < 1)
        {
            character.transform.position = GetLerpMovement(start, end, dir, step, t);
            t += Time.deltaTime * character.MovementSpeed;
            yield return null;
        }

        character.transform.position = end;
    }

    private Vector3 GetLerpMovement(Vector3 start, Vector3 end, Vector3 dir, float step, float t)
    {
        if (step == 0)
        {
            if (dir == Vector3.forward || dir == -Vector3.forward)
                return new Vector3(end.x, end.y, FLerp(start.z, end.z, t));
            return new Vector3(FLerp(start.x, end.x, t), end.y, end.z);
        }
        else if (step > 0)
        {
            if (dir == Vector3.forward || dir == -Vector3.forward)
                return new Vector3(end.x, FLerp(start.y, end.y, t, risingTerrainYCurve), FLerp(start.z, end.z, t));
            return new Vector3(FLerp(start.x, end.x, t), FLerp(start.y, end.y, t, risingTerrainYCurve), end.z);
        }

        if (dir == Vector3.forward || dir == -Vector3.forward)
            return new Vector3(end.x, FLerp(start.y, end.y, t, descendingTerrainYCurve, true), FLerp(start.z, end.z, t));
        return new Vector3(FLerp(start.x, end.x, t), FLerp(start.y, end.y, t, descendingTerrainYCurve, true), end.z);
    }

    private float FLerp(float a, float b, float t, AnimationCurve curve = null, bool mirrorY = false)
    {
        return a + (b - a) * (curve == null ? t : (mirrorY ? 1 - curve.Evaluate(t) : curve.Evaluate(t)));
    }

    #endregion
}
