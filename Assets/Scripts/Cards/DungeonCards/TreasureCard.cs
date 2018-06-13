public class TreasureCard : DungeonCard<TreasureCardData>
{
    public override void ExecuteTileSpawnEvent(GridTile tile)
    {
        Treasure treasure = Data.InstantiateEntity();
        Game.Dungeon.SpawnTreasure(treasure, tile);
    }
}
