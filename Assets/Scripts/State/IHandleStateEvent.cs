namespace Assets.Scripts.State
{
    public interface IHandleStateEvent<TEventType> where TEventType : struct
    {
        void HandleNewEvent(TEventType eventType, GameContext context);
    }
}
