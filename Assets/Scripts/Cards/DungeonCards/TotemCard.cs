public class TotemCard : DungeonSpawnCard<TotemCardData, CurseTotem>
{
    public override void ExecuteTileSpawnEvent(GridTile tile, DungeonCardExecutionContext context)
    {
        var totem = Data.InstantiateEntity();
        totem.StartCoroutine(totem.Data.CurseEffect.ApplyEffectOn(context.Player, totem.transform.position));
        totem.SpawnOnGrid(context.Dungeon, tile);
    }
}
