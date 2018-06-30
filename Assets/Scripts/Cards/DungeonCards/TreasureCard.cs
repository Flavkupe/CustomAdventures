public class TreasureCard : DungeonCard<TreasureCardData>
{
    public override void ExecuteTileSpawnEvent(GridTile tile, DungeonCardExecutionContext context)
    {
        var treasure = Data.InstantiateEntity();
        treasure.SpawnOnGrid(Game.Dungeon, tile);
    }
}
