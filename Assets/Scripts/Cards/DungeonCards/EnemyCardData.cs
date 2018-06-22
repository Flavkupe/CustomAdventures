using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Card", menuName = "Create Cards/Dungeon/Enemy Card", order = 1)]
public class EnemyCardData : EntityCardData<Enemy>
{
    public Sprite Sprite;

    public Stats BaseStats;

    public bool Boss = false;

    public int EXP = 1;

    public AudioClip[] DamagedSounds;
    public AudioClip[] DeathSounds;

    public AnimationEffectData DeathEffect;

    [Tooltip("List of possible sprites to leave on death, such as skeletons or blood.")]
    public Sprite[] LeaveOnDeath;

    public override DungeonCardType DungeonCardType { get { return DungeonCardType.Enemy; } }

    public BehaviorList Behavior;

    public override Type BackingCardType { get { return typeof(EnemyCard); } }

    public override Enemy InstantiateEntity()
    {
        var enemy = Utils.InstantiateOfType<Enemy>();
        enemy.Data = this;
        return enemy;
    }
}
