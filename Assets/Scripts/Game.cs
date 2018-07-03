﻿

public static class Game
{
    public static Player Player => Player.Instance;

    public static DeckManager Decks => DeckManager.Instance;

    public static Dungeon Dungeon => Dungeon.Instance;

    public static StateManager States => StateManager.Instance;

    public static UIManager UI => UIManager.Instance;

    public static CardDrawManager CardDraw => CardDrawManager.Instance;

    public static DebugManager Debug => DebugManager.Instance;

    public static SoundManager Sounds => SoundManager.Instance;
}

