using System;


public interface ITurnHandle
{
    #region Data
    public Action turnEnded { get; }
    #endregion


    #region Methods
    public void StartTurn();
    public void EndTurn();
    #endregion
}