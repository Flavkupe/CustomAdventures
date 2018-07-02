using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "Action", menuName = "Create Behavior/Actions/Use Ability On Player", order = 1)]
public class UseAbilityOnPlayerAction : ActorAction
{
    public StatusEffectData AbilityData;

    public override IEnumerator PerformAction(TileAI subject, GameContext context)
    {
        if (AbilityData == null)
        {
            Debug.Assert(false, "No AbilityData set!");
            yield break;
        }

        yield return subject.TwitchTowards(context.Player.transform.position);
        yield return AbilityData.ApplyEffectOn(context.Player, subject);
    }
}
