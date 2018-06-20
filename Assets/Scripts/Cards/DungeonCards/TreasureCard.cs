public class TreasureCard : DungeonCard<TreasureCardData>
{
    public override void ExecuteTileSpawnEvent(GridTile tile)
    {
        var treasure = Data.InstantiateEntity();
        treasure.SpawnOnGrid(Game.Dungeon, tile);
    }
}
