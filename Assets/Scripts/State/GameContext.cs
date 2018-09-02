using System;

public class GameContext
{
    public Func<DungeonActionType, bool> CanPerformAction;
    public Dungeon Dungeon;
    public Player Player;
    public UIManager UI;
}
