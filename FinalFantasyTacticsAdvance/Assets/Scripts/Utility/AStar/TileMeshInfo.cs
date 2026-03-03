public class TileMeshInfo
{
    #region Data
    public float[] costs;
    public int[] tilesIds;
    #endregion


    #region Methods
    public TileMeshInfo(float[] costs, int[] tilesIds)
    {
        this.costs = costs;
        this.tilesIds = tilesIds;
    }
    #endregion
}