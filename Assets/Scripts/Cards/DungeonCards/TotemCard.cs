using System.Collections;

public class TotemCard : DungeonSpawnCard<TotemCardData, CurseTotem>
{
    protected override IEnumerator ExecuteSpawnEvent(GridTile tile, DungeonCardExecutionContext context)
    {
        var totem = Data.InstantiateEntity();
        totem.StartCoroutine(totem.Data.CurseEffect.ApplyEffectOn(context.Player, totem.transform.position));
        totem.SpawnOnGrid(context.Dungeon, tile);
        yield return null;
    }
}
