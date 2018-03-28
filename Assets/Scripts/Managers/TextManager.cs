using JetBrains.Annotations;

public class TextManager : SingletonObject<TextManager>
{
    public FloatyText DamageTextTemplate;

    [UsedImplicitly]
    private void Awake()
    {
        Instance = this;
    }
}
