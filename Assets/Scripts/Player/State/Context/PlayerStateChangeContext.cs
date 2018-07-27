namespace Assets.Scripts.Player.State.Context
{
    public class PlayerStateChangeContext
    {
        public PlayerStateChangeContext(PlayerEventType eventType, GameContext context)
        {
            EventType = eventType;
            GameContext = context;
        }

        public PlayerEventType EventType { get; }
        public GameContext GameContext { get; }
    }
}