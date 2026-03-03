using System;
using UnityEngine;


public class NodeLink : IComparable<NodeLink>
{
    #region Data
    Node tileA;
    Node tileB;
    float weight;
    float stepHeight;
    E_WorldTileLinkDirection direction; //-1 to tileA, 0 for both directions, 1 to tileB


    public float Weight => weight;
    public float StepHeight => stepHeight;
    public E_WorldTileLinkDirection Direction => direction;
    #endregion


    #region Methods
    public NodeLink(Node tileA, Node tileB, float weight, float stepHeight, E_WorldTileLinkDirection dir = E_WorldTileLinkDirection.BOTH)
    {
        if (tileA == null || tileB == null)
        {
            Debug.LogError("Both WorldTileLink's WorldTiles have been set to null!");
            return;
        }

        if (tileA == tileB)
        {
            Debug.LogError("The WorldTileLink can't be a loop!");
            return;
        }

        this.tileA = tileA;
        this.tileB = tileB;
        this.weight = weight;
        this.stepHeight = stepHeight;

        direction = dir;
    }


    public void SetWeight(float weight)
    {
        this.weight = weight;
    }


    public void SetStepHeight(float stepHeight)
    {
        this.stepHeight = stepHeight;
    }


    public void SetDirection(E_WorldTileLinkDirection dir)
    {
        direction = dir;
    }


    public Node GetNeighbor(Node tile)
    {
        if (tile == tileA && direction >= 0)
            return tileB;
        else if (tile == tileB && direction <= 0)
            return tileA;

        return null;
    }


    public bool ContainsTile(Node tile)
    {
        if (tile == null)
        {
            Debug.LogError("The given WorldTile to the WorldTileLink's ContainsNode method must not be null!");
            return false;
        }

        if (tileA == tile || tileB == tile)
            return true;
        return false;
    }


    public bool UnbindFromTiles()
    {
        if (tileA == null || tileB == null)
        {
            Debug.LogError("One or more WorldTiles have been discovered null while unbinding the WorldTileLink!");
            return false;
        }
        else
        {
            tileA.RemoveLinksToTile(tileB);
            tileB.RemoveLinksToTile(tileA);
            return true;
        }
    }

    public int CompareTo(NodeLink other)
    {
        return weight > other.Weight ? 1 : -1;
    }
    #endregion
}
