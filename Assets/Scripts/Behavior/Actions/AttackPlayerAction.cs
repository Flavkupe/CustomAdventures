using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "Action", menuName = "Create Behavior/Actions/Attack Player", order = 1)]
public class AttackPlayerAction : ActorAction
{
    public override IEnumerator PerformAction(TileAI subject, GameContext context)
    {
        yield return subject.TwitchTowards(context.Player.transform.position);
        context.Player.DoDamage(subject.CurrentStats.BaseStrength);
    }
}

