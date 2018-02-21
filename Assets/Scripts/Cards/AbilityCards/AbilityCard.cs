
using System;

public abstract class AbilityCard<T> : Card<T>, IAbilityCard where T : AbilityCardData
{
    public override CardType CardType { get { return CardType.Ability; } }
    public AbilityCardType AbilityCardType { get { return this.Data.AbilityCardType; } }

    public AbilityEventType AbilityEventType { get { return Data.AbilityEventType; } }

    public virtual void ActivateAbility()
    {
        // Must implement to use!
        throw new NotImplementedException();
    }

    public virtual void ActivateAbility(Tile target)
    {
        // Must implement to use!
        throw new NotImplementedException();
    }

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

    void ActivateAbility();
    void ActivateAbility(Tile target);
}

public enum AbilityActivationType
{
    Instant,
    TargetTile,
}

public enum AbilityCardType
{
    Attack,
    SelfBuff,
}

public enum AbilityEventType
{
    AttackAdjacent
}

public abstract class AbilityCardData : CardData
{
    public abstract AbilityCardType AbilityCardType { get; }
    public AbilityEventType AbilityEventType;
    public AbilityActivationType ActivationType;
}