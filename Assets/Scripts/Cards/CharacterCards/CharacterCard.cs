public abstract class CharacterCard<T> : Card<T>, ICharacterCard where T : CharacterCardData
{
    public override CardType CardType { get { return CardType.Character; } }
    public CharacterCardType CharacterCardType { get { return Data.CharacterCardType; } }

    public abstract void ApplyEffect();

    // Use this for initialization
    private void Start ()
    {
	}

    // Update is called once per frame
    private void Update ()
    {
	}
}

public interface ICharacterCard : ICard
{
    CharacterCardType CharacterCardType { get; }

    void ApplyEffect();
}

public enum CharacterCardType
{
    AttributeGain,
    AbilityGain
}

public abstract class CharacterCardData : CardData
{
    public abstract CharacterCardType CharacterCardType { get; }
}