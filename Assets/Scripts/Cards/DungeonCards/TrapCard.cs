using System.Collections;
using System.Linq;

public class TrapCard : DungeonSpawnCard<TrapCardData, TileTrap>
{
    protected override IEnumerator ExecuteSpawnEvent(GridTile tile, DungeonCardExecutionContext context)
    {
        if (tile.GetPassableTileEntities().All(a => a.EntityType != TileEntityType.Trap))
        {
            var trap = Data.InstantiateEntity();
            trap.SpawnOnGrid(Game.Dungeon, tile);
        }
        else
        {
            // TODO: if contains a trap already, spawn elsewhere
        }

        yield return null;
    }

    public override bool RequiresFullTile => false;
}
