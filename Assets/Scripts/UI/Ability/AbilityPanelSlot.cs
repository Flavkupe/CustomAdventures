using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class AbilityPanelSlot : MonoBehaviour
{    
    public Image IconImage;
    public Sprite LockIcon;
    private IAbilityCard ability;
        
    public IAbilityCard Ability { get { return ability; } }

    public bool IsEmpty { get { return ability == null; } }

    public float HoverTimer = 1.0f;
    private float _timer;
    private bool _hovering = false;

    public void SetAbility(IAbilityCard ability)
    {
        // TODO: Game.Decks
        var fullSize = Game.Decks.DeckBigSize;

        this.ability = ability;
        SetSpriteImage(ability.AbilityIcon);
        
        if (this.ability.Object != null)
        {
            var rectTransform = GetComponent<RectTransform>();
            this.ability.Object.transform.SetParent(Game.Player.transform);

            var propX = rectTransform.transform.position.x;
            var propY = rectTransform.transform.position.y;

            // TODO: this is a shitty way to do it; create a clone of this instead!
            this.ability.Object.transform.position = new Vector3(propX, propY + 3.0f, -5.0f);
            this.ability.SetFaceUp();
            this.ability.Object.gameObject.SetActive(false);
            this.ability.Object.transform.localScale = new Vector3(fullSize, fullSize, fullSize);
        }
    }

    private void SetSpriteImage(Sprite sprite)
    {
        IconImage.sprite = sprite;
        IconImage.gameObject.SetActive(true);
    }

    public void SetLocked()
    {
        SetSpriteImage(LockIcon);
    }

    public void Clear()
    {
        ability = null;
        IconImage.sprite = null;
        IconImage.gameObject.SetActive(false);
    }

    public void OnHoverEnter()
    {
        _hovering = true;        
    }

    public void OnHoverExit()
    {
        _hovering = false;
        _timer = HoverTimer;
        if (ability != null && ability.Object != null)
        {
            ability.Object.gameObject.SetActive(false);
        }
    }

    public void OnClick()
    {
        if (ability != null && Game.Player.PlayerHasActions)
        {
            Game.Player.UseAbility(ability);
        }
    }

    public void Update()
    {
        if (_hovering && _timer > 0.0f && ability != null && ability.Object != null)
        {
            _timer -= Time.deltaTime;
            if (_timer <= 0.0f)
            {
                ability.Object.gameObject.SetActive(true);
            }
        }
    }

    private void Awake()
    {
        _timer = HoverTimer;
        IconImage.gameObject.SetActive(false);
        Debug.Assert(IconImage != null, "Must have IconImage set!");
    }        
}

