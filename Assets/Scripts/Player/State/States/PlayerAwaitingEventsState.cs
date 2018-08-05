
using Assets.Scripts.Player.State.Context;
using UnityEngine;

public class PlayerAwaitingEventsState : PlayerState
{
    private PlayerEventType _evenType;
    
    public PlayerAwaitingEventsState(PlayerEventType eventType, PlayerStateController controller) : base(controller)
    {
        _evenType = eventType;
        AddTransition(new PlayerReturnStateTransition(PlayerDecision.Decisions.DidEventOccur(_evenType), controller)); 
    }

    public override bool CanPerformAction(DungeonActionType actionType)
    {
        switch (actionType)
        {
            case DungeonActionType.PlayerMove:
            case DungeonActionType.EntityMove:
            case DungeonActionType.OpenMenu:
            case DungeonActionType.UseItem:
            default:
                return false;
        }
    }

    public override void StateEntered(IState<PlayerStateChangeContext> previousState, PlayerStateChangeContext context)
    {
        base.StateEntered(previousState, context);
        Debug.Log($"Awaiting event {_evenType.ToString()}");
    }
}
