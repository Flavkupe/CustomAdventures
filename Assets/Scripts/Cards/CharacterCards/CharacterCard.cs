using System.Collections;

public abstract class CharacterCard<T> : Card<T>, ICharacterCard where T : CharacterCardData
{
    public override CardType CardType { get { return CardType.Character; } }
    public CharacterCardType CharacterCardType { get { return Data.CharacterCardType; } }

    public abstract IEnumerator ApplyEffect(CharacterCardExecutionContext context);

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

    IEnumerator ApplyEffect(CharacterCardExecutionContext context);
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

public class CharacterCardExecutionContext
{
    public Player Player { get; private set; }
    
    public DeckManager Decks { get; private set; }

    public CharacterCardExecutionContext(Player player, DeckManager decks)
    {
        Player = player;
        Decks = decks;
    }
}