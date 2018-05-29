using System.Linq;

public class TrapCard : DungeonCard<TrapCardData>
{
    public override void ExecuteTileSpawnEvent(GridTile tile)
    {
        if (!tile.GetPassableTileEntities().Any(a => a.EntityType == TileEntityType.Trap))
        {            
            var trap = InstantiateTrap();
            Game.Dungeon.SpawnTrap(trap, tile);
        }
        else
        {
            // TODO: if contains a trap already, spawn elsewhere
        }
    }

    private TileTrap InstantiateTrap()
    {                
        TileTrap trap = InstantiateOfType<TileTrap>();
        trap.Data = Data;
        return trap;
    }

    public override bool RequiresFullTile { get { return false; } }
}
