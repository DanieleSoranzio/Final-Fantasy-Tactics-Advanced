using System.Collections.Generic;
using UnityEngine;
using Debug = UnityEngine.Debug;


public class NodeGraph
{
    #region Data
    List<Node> tiles;


    public int Count => tiles.Count;
    #endregion


    #region Methods
    public NodeGraph()
    {
        tiles = new List<Node>();
    }


    public bool AddTile(Node tile)
    {
        if (tile == null)
        {
            Debug.LogError("Can't add a null WorldTile to the WorldGraph!");
            return false;
        }

        if (tiles.Contains(tile))
            return false;

        tile.SetId(tiles.Count);
        tiles.Add(tile);
        return true;
    }


    public bool RemoveTile(Node tile)
    {
        if (tile == null)
        {
            Debug.LogError("Can't remove a null WorldTile from the WorldGraph!");
            return false;
        }

        if (!tiles.Contains(tile))
            return false;

        bool decreaseId = false;
        for (int i = 0; i < tiles.Count; i++)
        {
            if (decreaseId)
                tiles[i].SetId(i - 1);

            if (tiles[i] == tile)
                decreaseId = true;
        }

        tile.ClearLinks();
        tiles.Remove(tile);
        return true;
    }


    public bool AddLink(Node tileA, Node tileB, float weight, E_WorldTileLinkDirection dir = E_WorldTileLinkDirection.BOTH)
    {
        if (tileA == null || tileB == null)
        {
            Debug.Log("WorldTiles can't be null while creating new WorldTileLinks!");
            return false;
        }

        if (!tiles.Contains(tileA) || !tiles.Contains(tileB))
        {
            Debug.Log("One or both of the specified WorldTiles of the WorldTileLink is not contained in the WorldGraph!");
            return false;
        }

        NodeLink link = new NodeLink(tileA, tileB, weight, tileA.CalculateStepHeight(tileB, dir), dir);
        return tileA.AddLink(link) && tileB.AddLink(link);
    }


    public bool RemoveLink(Node tileA, Node tileB)
    {
        if (tileA == null || tileB == null)
        {
            Debug.Log("WorldTiles can't be null while removing WorldTileLinks!");
            return false;
        }

        if (!tiles.Contains(tileA) || !tiles.Contains(tileB))
        {
            Debug.Log("One or both of the specified WorldTiles of the WorldTileLink is not contained in the WorldGraph!");
            return false;
        }

        tileA.RemoveLinksToTile(tileB);
        tileB.RemoveLinksToTile(tileA);
        return true;
    }


    public Node[] GetArea(in Node start, float range, float jump = float.MaxValue, bool filterOccupiedTiles = true,bool isAttacking=false)
    {
        //Perform security checks first
        if (start == null)
        {
            Debug.Log("WorldTiles can't be null while requesting a valid area!");
            return null;
        }

        if (!tiles.Contains(start))
        {
            Debug.Log("The specified starting WorldTile is not contained in the WorldGraph while providing a valid area!");
            return null;
        }

        //Get all area tiles ids
        float Jump = jump==float.MaxValue ? float.MaxValue:jump/2;
        TileMeshInfo info = AreaSearch(start, range, Jump, isAttacking);

        //Convert area tiles ids to references and return them
        List<Node> areaTiles = new List<Node>();
        for (int i = 0; i < info.tilesIds.Length; i++)
        {
            if (filterOccupiedTiles && tiles[info.tilesIds[i]].CheckIfOccupied())
                continue;
            areaTiles.Add(tiles[info.tilesIds[i]]);
        }

        return areaTiles.ToArray();
    }


    private TileMeshInfo AreaSearch(in Node start, float range, float jump, bool isAttack)
    {
        PriorityQueue<int> availableTiles = new PriorityQueue<int>();  //PriorityQueue containing all tile ids yet to be visited
        HashSet<int> exploredTiles = new HashSet<int>();  //HashSet containing all explored tiles ids (complexity O(1))
        List<int> areaTiles = new List<int>();  //List containing all tile ids in range from the starting tile
        float[] costsFromStart = new float[tiles.Count];  //Array containing the lowest total cost from the current tile to the starting tile
        bool attacking = isAttack;
        //Initialize all costsFromStart costs as worst case scenario
        for (int i = 0; i < costsFromStart.Length; i++)
            costsFromStart[i] = float.MaxValue;

        //Enqueue the starting tile
        costsFromStart[start.Id] = 0;
        availableTiles.Enqueue(start.Id, 0);

        //While at least a tile is available, keep moving towards the lowest costing one first
        Node currentTile;
        Node connectedTile;
        NodeLink connectionLink;
        float connectedTileCost;
        while (availableTiles.Count > 0)
        {
            //Get the next tile to be explored
            currentTile = tiles[availableTiles.Dequeue()];

            //If the current tile has not already been explored, add it to the explored list and start checking its links
            if (exploredTiles.Contains(currentTile.Id))
                continue;

            exploredTiles.Add(currentTile.Id);

            //Since all links starting from a tile will weight the same, no sorting is required
            for (int i = 0; i < currentTile.LinksCount; i++)
            {
                connectionLink = currentTile.Links[i];
                connectedTile = connectionLink.GetNeighbor(currentTile);

                //Get the adjacient tile and check if it has not already been explored or if it's reachable
                if (!exploredTiles.Contains(connectedTile.Id) && jump >= connectionLink.StepHeight && -jump <= connectionLink.StepHeight)
                {
                    if(!attacking)
                    {
                        connectedTileCost = costsFromStart[currentTile.Id] + connectionLink.Weight;
                       
                    }
                    else
                    {
                        connectedTileCost = costsFromStart[currentTile.Id] + 1;
                    }

                    //If the tile is in range, try updating the cost (if in a better path) for the tile to be reached from the start
                    if (connectedTileCost <= range)
                    {
                        costsFromStart[connectedTile.Id] = Mathf.Min(costsFromStart[connectedTile.Id], connectedTileCost);

                        //If the tile wasn't already part of the final area add it and mark it as explorable
                        if (!areaTiles.Contains(connectedTile.Id))
                        {
                            areaTiles.Add(connectedTile.Id);
                            availableTiles.Enqueue(connectedTile.Id, costsFromStart[connectedTile.Id]);
                        }
                    }
                }
            }
        }

        return new TileMeshInfo(costsFromStart, areaTiles.ToArray());
    }


    public Node[] GetPath(in Node start, in Node end, float jump  = float.MaxValue)
    {
        //Perform security checks first
        if (start == null || end == null)
        {
            Debug.Log("WorldTiles can't be null while requesting a valid path!");
            return null;
        }

        if (!tiles.Contains(start) || !tiles.Contains(end))
        {
            Debug.Log("One or both of the specified WorldTiles is not contained in the WorldGraph while providing a valid path!");
            return null;
        }

        //Search a valid path between start tile and end tile using AStar algorithm
        TileMeshInfo info = AStarPathSearch(start, end, jump);

        if (info == null)
        {
            Debug.LogWarning($"No valid path between WorldTiles {start} -> {end} has been found!");
            return null;
        }

        //Recreate path from end to start and reverse it before returning it (start to end).
        int currentTileId = end.Id;
        int nextTileId = info.tilesIds[currentTileId];

        Stack<Node> path = new Stack<Node>();

        while (currentTileId != start.Id)
        {
            path.Push(tiles[currentTileId]);
            currentTileId = nextTileId;
            nextTileId = info.tilesIds[nextTileId];
        }

        //Push in the stack the last tile (the start)
        path.Push(tiles[start.Id]);

        return path.ToArray();
    }


    private TileMeshInfo AStarPathSearch(in Node start, in Node end, float jump)
    {
        PriorityQueue<int> availableTiles = new PriorityQueue<int>();  //PriorityQueue containing all tiles yet to be visited
        HashSet<int> exploredTiles = new HashSet<int>();  //HashSet containing all explored tiles ids (complexity O(1))
        float[] costsFromStart = new float[tiles.Count];  //Array containing the lowest total cost from the current tile to the starting tile
        int[] bestPreviousTileId = new int[tiles.Count];  //Array containing a tile id reference indicating which tile was the current one coming from

        //Initialize all costsFromStart costs as worst case scenario
        for (int i = 0; i < costsFromStart.Length; i++)
            costsFromStart[i] = float.MaxValue;

        //Initialize PriorityQueue with starting tile
        costsFromStart[start.Id] = 0;
        availableTiles.Enqueue(start.Id, 0, 0);

        Node currentTile;
        Node connectedTile;
        NodeLink connectionLink;
        float heuristic;
        float connectionCost;
        while (availableTiles.Count > 0)
        {
            currentTile = tiles[availableTiles.Dequeue()];

            //If the currentTile is the ending one, return all the tiles costs and the tiles path ids
            if (currentTile == end)
                return new TileMeshInfo(costsFromStart, bestPreviousTileId);

            //Else mark it as explored and start exploring its neighbors
            exploredTiles.Add(currentTile.Id);

            for (int i = 0; i < currentTile.LinksCount; i++)
            {
                //For each neighbor check if it has already been explored
                connectionLink = currentTile.Links[i];
                connectedTile = connectionLink.GetNeighbor(currentTile);

                if (exploredTiles.Contains(connectedTile.Id))
                    continue;

                //Check if the tile is reachable from the current position
                if (jump >= connectionLink.StepHeight && -jump <= connectionLink.StepHeight)
                {
                    //If the neighbor has not been explored, calculate its new connection cost
                    connectionCost = costsFromStart[currentTile.Id] + connectionLink.Weight;

                    //If the new connection cost is better, update costsFromStart and bestPreviousTileId
                    if (costsFromStart[connectedTile.Id] > connectionCost)
                    {
                        costsFromStart[connectedTile.Id] = connectionCost;
                        bestPreviousTileId[connectedTile.Id] = currentTile.Id;

                        //Calculate the AStar heuristic and use it in the priority queue to move to tiles closer to the end faster
                        heuristic = connectedTile.GetManhattanDistance(end);
                        availableTiles.Enqueue(connectedTile.Id, connectionCost + heuristic, heuristic);
                    }
                }
            }
        }

        //No path has been found
        return null;
    }
    public List<Node> GetOccupiedTiles()
    {
        List<Node> occupiedTiles = new List<Node>();
        foreach (var tile in tiles)
        {
            if (tile != null)
            {
                if (tile.CheckIfOccupied())
                    occupiedTiles.Add(tile);
            }
        }
        return occupiedTiles;   
    }
    public List<Node> GetEnemyTile(int layerEnemy)
    {
        List<Node> enemyTiles = new List<Node>();
        foreach (var tile in tiles)
        {
            if (tile != null)
            {
                if (tile.CheckIfOccupied()&&tile.LayerOfTheOccupieing==layerEnemy)
                    enemyTiles.Add(tile);
            }
        }
        return enemyTiles;
    }

    public void PrintGraph()
    {
        string print = "";

        for (int i = 0; i < tiles.Count; i++)
            print += $"{tiles[i].PrintLinks()}\n";

        Debug.Log(print);
    }
    #endregion
}
