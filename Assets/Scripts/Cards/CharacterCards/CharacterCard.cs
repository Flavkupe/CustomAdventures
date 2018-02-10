public abstract class CharacterCard<T> : Card<T>, ICharacterCard where T : CharacterCardData
{
    public override CardType CardType { get { return CardType.Character; } }
    public abstract CharacterCardType CharacterCardType { get; }

    // Use this for initialization
    void Start ()
    {
	}

	// Update is called once per frame
	void Update ()
    {
	}
}

public interface ICharacterCard : ICard
{
    CharacterCardType CharacterCardType { get; }
}

public enum CharacterCardType
{
    AttributeGain
}

public abstract class CharacterCardData : CardData
{
    public abstract CharacterCardType CharacterCardType { get; }
}