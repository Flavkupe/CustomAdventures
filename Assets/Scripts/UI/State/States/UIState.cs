namespace Assets.Scripts.UI.State.States
{
    public class UIStateTransition : Transition<UIEventType>
    {
        public UIStateTransition(IDecision<UIEventType> decision, IState<UIEventType> next)
            : base(decision, next)
        {
        }
    }

    public abstract class UIState : State<UIEventType>, IActionDeterminant<DungeonActionType>
    {
        protected UIState(IStateController<UIEventType> contoller) : base(contoller)
        {
        }

        public abstract bool CanPerformAction(DungeonActionType actionType);
    }
}
