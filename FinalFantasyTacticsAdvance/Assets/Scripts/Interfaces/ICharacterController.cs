public interface ICharacterController
{
    #region Data
    public EntityBehaviour ActivePawn { get; }
    #endregion


    #region Methods
    public void Select(EntityBehaviour character);
    public void Unselect();
    #endregion
}