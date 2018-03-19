

public static class Game
{
    public static Player Player { get { return Player.Instance; } }

    public static DeckManager Decks { get { return DeckManager.Instance; } }

    public static DungeonManager Dungeon { get { return DungeonManager.Instance; } }

    public static StateManager States { get { return StateManager.Instance; } }

    public static UIManager UI { get { return UIManager.Instance; } }

    public static CardDrawManager CardDraw { get { return CardDrawManager.Instance; } }

    public static EffectsManager Effects { get { return EffectsManager.Instance; } }

    public static DebugManager Debug { get { return DebugManager.Instance; } }
}

