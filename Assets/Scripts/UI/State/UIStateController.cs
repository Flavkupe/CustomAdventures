using Assets.Scripts.UI.State.Decisions;
using Assets.Scripts.UI.State.States;

namespace Assets.Scripts.UI.State
{
    public class UIStateController : StateController<UIState, UIEventType>, IActionDeterminant<DungeonActionType>
    {
        public UIStateController() : base("UI")
        {
            var idleState = new IdleUIState(this);
            var blockingUIState = new BlockingUIOpenState(this);

            idleState.AddTransitions(new[]
            {
                new UIStateTransition(UIDecision.Decisions.DidEventOccur(UIEventType.InterfaceOpened), blockingUIState),
                new UIStateTransition(UIDecision.Decisions.DidEventOccur(UIEventType.DialogShown), blockingUIState),
            });

            blockingUIState.AddTransitions(new[]
            {
                new UIStateTransition(UIDecision.Decisions.DidEventOccur(UIEventType.InterfaceClosed), idleState),
                new UIStateTransition(UIDecision.Decisions.DidEventOccur(UIEventType.DialogClosed), idleState),
            });

            FirstState = idleState;
            CurrentState = FirstState;
        }

        public bool CanPerformAction(DungeonActionType actionType)
        {
            return CanPerformActionInState(actionType);
        }
    }
}
