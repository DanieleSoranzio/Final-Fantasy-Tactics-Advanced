using CustomSorting;
using System.Collections.Generic;
using UnityEngine;


public class Node : MonoBehaviour
{
    #region Data
    List<NodeLink> links;
    Material highlightMat;


    int id;
    bool occupied;
    bool highlighted;
    int entityOccupieing;


    [Header("Settings")]
    [SerializeField, Min(1)] float cost;
    [Space(10), SerializeField] LayerMask sampleMask;
    [Header("References")]
    [SerializeField] GameObject highlight;


    public List<NodeLink> Links => links;
    public int LinksCount => links.Count;
    public int Id => id;
    public bool Occupied => occupied;
    public float Cost => cost;
    public bool Highlighted => highlighted; 
    public int LayerOfTheOccupieing => entityOccupieing;
    #endregion


    #region Mono
    private void Awake()
    {
        links = new List<NodeLink>();

        highlight.SetActive(false);
        highlightMat = highlight.GetComponent<MeshRenderer>().material;
    }


    private void Start()
    {
        RegisterToTileMesh();
        occupied=CheckIfOccupied();
    }
    #endregion


    #region Methods
    public void SetId(int id)
    {
        this.id = id;
    }


    public NodeLink GetLinkToTile(Node tile)
    {
        for (int i = 0; i < links.Count; i++)
        {
            if (links[i].GetNeighbor(tile) != null)
                return links[i];
        }

        return null;
    }


    public bool AddLink(NodeLink link)
    {
        if (link == null)
        {
            Debug.LogError("Can't add a null WorldTileLink to a WorldTile!");
            return false;
        }

        if (GetLinkToTile(link.GetNeighbor(this)) != null)
            return false;

        links.Add(link);
        return true;
    }


    public bool AddLinkToTile(Node tile, float weight, E_WorldTileLinkDirection dir = E_WorldTileLinkDirection.BOTH)
    {
        if (tile == null)
        {
            Debug.LogError("Can't add a WorldTileLink with a null WorldTile!");
            return false;
        }

        NodeLink link = new NodeLink(this, tile, weight, CalculateStepHeight(tile, dir), dir);
        tile.AddLink(link);

        if (GetLinkToTile(link.GetNeighbor(this)) != null)
            return false;

        links.Add(link);
        return true;
    }


    public bool RemoveLink(NodeLink link)
    {
        if (link == null)
        {
            Debug.LogError("Can't remove a null WorldTileLink from a WorldTile!");
            return false;
        }

        if (!links.Contains(link))
            return false;

        return link.UnbindFromTiles();
    }


    public void RemoveLinksToTile(Node tile)
    {
        List<NodeLink> remainingLinks = new List<NodeLink>();

        for (int i = 0; i < links.Count; i++)
        {
            if (!links[i].ContainsTile(tile))
                remainingLinks.Add(links[i]);
        }

        links = remainingLinks;
    }


    public void ClearLinks()
    {
        for (int i = 0; i < links.Count; i++)
            links[i].GetNeighbor(this).RemoveLinksToTile(this);
    }


    public void SortLinks()
    {
        links.MergeSort();
    }


    public string PrintLinks()
    {
        string print = $"WorldTile {name} ({links.Count} links):\n";

        for (int i = 0; i < links.Count; i++)
            print += $"\t{links[i].GetNeighbor(this).name} (w{links[i].Weight}, sh{links[i].StepHeight}, d{(int)links[i].Direction})\n";

        return print;
    }


    public float GetManhattanDistance(Node tile)
    {
        return Mathf.Abs(transform.position.x - tile.transform.position.x) + Mathf.Abs(transform.position.z - tile.transform.position.z);
    }


    public override string ToString()
    {
        return name.ToString();
    }


    private void RegisterToTileMesh()
    {
        BindToNeighbor(Vector3.forward);
        BindToNeighbor(Vector3.right);
        BindToNeighbor(Vector3.back);
        BindToNeighbor(Vector3.left);

        NodeManager.Instance.RegisterTile(this);
    }


    private void BindToNeighbor(Vector3 direction)
    {
        if (Physics.Raycast(transform.position + direction + Vector3.up * float.MaxValue / 2,
            Vector3.down, out RaycastHit hit, float.MaxValue, sampleMask))
        {
            if (hit.transform.gameObject.TryGetComponent(out Node tile))
                links.Add(new NodeLink(this, tile, cost, CalculateStepHeight(tile, E_WorldTileLinkDirection.A_TO_B),
                    E_WorldTileLinkDirection.A_TO_B));
        }
    }


    public float CalculateStepHeight(Node tileB, E_WorldTileLinkDirection dir)
    {
        switch (dir)
        {
            case E_WorldTileLinkDirection.B_TO_A:
                return transform.position.y - tileB.transform.position.y;

            case E_WorldTileLinkDirection.BOTH:
                return Mathf.Min(Mathf.Abs(transform.position.y - tileB.transform.position.y),
                    Mathf.Abs(tileB.transform.position.y - transform.position.y));

            case E_WorldTileLinkDirection.A_TO_B:
                return tileB.transform.position.y - transform.position.y;
        }

        return 0f;
    }

    public bool CheckIfOccupied()
    {
        if (Physics.Raycast(transform.position+Vector3.down, Vector3.up*2, out RaycastHit enemyFound, 2f))
        {
            Debug.Log("Occupied is true " + enemyFound.collider.gameObject.layer);
            entityOccupieing = enemyFound.collider.gameObject.layer;
            Debug.Log(entityOccupieing);
            occupied = true;
            return occupied;
        }
        occupied=false;
        return occupied;
    }
    public EntityBehaviour EntityOnTile()
    {
        EntityBehaviour target=null;
        if(occupied)
        {
            if (Physics.Raycast(transform.position + Vector3.down, Vector3.up * 2, out RaycastHit enemyFound, 2f))
            {
                target = enemyFound.collider.gameObject.GetComponentInParent<EntityBehaviour>();
                return target;
            }
        }
        return target;
    }

    public void ToggleHighlight(bool enable, Color? color = null)
    {
        highlighted = enable;
        if (enable)
        {
            if (color != null)
                highlightMat.color = new Color(color.Value.r, color.Value.g, color.Value.b, color.Value.a);
            else
                highlightMat.color=Color.white;

        }

        highlight.SetActive(enable);

    }
    #endregion
}
