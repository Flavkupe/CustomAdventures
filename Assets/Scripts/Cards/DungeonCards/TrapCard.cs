using System.Linq;

public class TrapCard : DungeonCard<TrapCardData>
{
    public override void ExecuteTileSpawnEvent(GridTile tile)
    {
        if (!tile.GetPassableTileEntities().Any(a => a.EntityType == TileEntityType.Trap))
        {            
            var trap = Data.InstantiateEntity();
            trap.SpawnOnGrid(Game.Dungeon, tile);
        }
        else
        {
            // TODO: if contains a trap already, spawn elsewhere
        }
    }

    public override bool RequiresFullTile { get { return false; } }
}
