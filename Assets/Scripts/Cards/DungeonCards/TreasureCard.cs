public class TreasureCard : DungeonCard<TreasureCardData>
{
    public override void ExecuteTileSpawnEvent(GridTile tile)
    {
        Treasure treasure = InstantiateTreasure();
        Game.Dungeon.SpawnTreasure(treasure, tile);
    }

    private Treasure InstantiateTreasure()
    {
        Treasure treasure = InstantiateOfType<Treasure>();
        treasure.Data = Data;
        return treasure;
    }
}
