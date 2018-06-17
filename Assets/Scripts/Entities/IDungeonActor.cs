using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAIDungeonActor : IDungeonActor
{
    IEnumerator ProcessCharacterTurn();
}

public interface IDungeonActor
{
    void AfterAppliedStatusEffect(StatusEffectData effect);

    Stats CurrentStats { get; }

    List<StatusEffect> Effects { get; }

    void DoDamage(int damage);

    void DoHealing(int healing);

    Transform Transform { get; }
}
