using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbilityCard<T> : Card<T>, IAbilityCard where T : AbilityCardData
{
    public override CardType CardType { get { return CardType.Ability; } }
    public abstract AbilityCardType AbilityCardType { get; }

    public AbilityEventType AbilityEventType { get { return Data.AbilityEventType; } }

    // Use this for initialization
    void Start ()
    {
	}

	// Update is called once per frame
	void Update ()
    {
	}
}

public interface IAbilityCard : ICard
{
    AbilityCardType AbilityCardType { get; }
    AbilityEventType AbilityEventType { get; }
}

public enum AbilityCardType
{
    Attack,
}

public enum AbilityEventType
{
    AttackAdjacent
}

public abstract class AbilityCardData : CardData
{
    public abstract AbilityCardType AbilityCardType { get; }
    public AbilityEventType AbilityEventType;
}