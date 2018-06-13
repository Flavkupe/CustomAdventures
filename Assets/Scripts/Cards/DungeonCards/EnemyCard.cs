public class EnemyCard : DungeonCard<EnemyCardData>
{
    public override void ExecuteTileSpawnEvent(GridTile tile)
    {
        Enemy enemy = Data.InstantiateEntity();
        Game.Dungeon.SpawnEnemy(enemy, tile);
    }
}
