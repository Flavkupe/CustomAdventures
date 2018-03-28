public class EnemyCard : DungeonCard<EnemyCardData>
{
    public override void ExecuteTileSpawnEvent(GridTile tile)
    {
        Enemy enemy = InstantiateEnemy();
        Game.Dungeon.SpawnEnemy(enemy, tile);
    }

    public Enemy InstantiateEnemy()
    {
        Enemy enemy = InstantiateOfType<Enemy>();
        enemy.Data = Data;
        return enemy;
    }
}
