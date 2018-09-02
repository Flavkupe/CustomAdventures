
using Assets.Scripts.State;
using UnityEngine;

/// <summary>
/// State in which a specific event is being awaited, which should block all actions
/// </summary>
public class PlayerAwaitingEventsState : PlayerState
{
    private readonly PlayerEventType _evenType;
    
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

    public override void StateEntered(IState<PlayerEventType> previousState, StateContext<PlayerEventType> context)
    {
        base.StateEntered(previousState, context);
        Debug.Log($"Awaiting event {_evenType.ToString()}");
    }
}
