using System;
using System.Collections.Generic;

public static class EventManager 
{
    //UI Buttons
    public static Action moveSelected;
    public static Action attackSelected;
    public static Action<int> attackChosen;
    public static Action statusSelected;
    public static Action waitSelected;
    public static Action backSelected;
    public static Action<Stats, Weapon> OnStatus;
    public static Action GameOver;
    public static Action GameWon;
    
    //Ui Updates
    public static Action<List<Attack>> updateAttackListUI;
    public static Action<bool,bool> OnStartAction;
    public static Action OnEndTurn;
    public static Action startedMoving;
    public static Action startedAttacking;
    public static Action<List<EntityBehaviour>> newTurnList;
}
