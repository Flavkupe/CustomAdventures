

public static class Game
{
    public static Player Player => Player.Instance;

    public static DeckManager Decks => DeckManager.Instance;

    public static Dungeon Dungeon => Dungeon.Instance;

    public static UIManager UI => UIManager.Instance;

    public static DebugManager Debug => DebugManager.Instance;

    public static SoundManager Sounds => SoundManager.Instance;

    public static TokenManager Tokens => TokenManager.Instance;

    public static World World => World.Instance;
}

