
using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class AbilityPanelSlot : MonoBehaviour
{    
    public Image IconImage;
    private IAbilityCard ability;
        
    public IAbilityCard Ability { get { return ability; } }

    public bool IsEmpty { get { return this.ability == null; } }

    public void SetAbility(IAbilityCard ability)
    {
        this.ability = ability;
        this.IconImage.sprite = ability.AbilityIcon;
        this.IconImage.gameObject.SetActive(true);
        
        if (this.ability.Object != null)
        {
            var rectTransform = GetComponent<RectTransform>();
            this.ability.Object.transform.SetParent(Game.Player.transform);

            var propX = rectTransform.transform.position.x / Camera.main.pixelWidth;
            var propY = rectTransform.transform.position.y / Camera.main.pixelHeight;
            
            Vector3 worldPosition = Camera.main.ViewportToWorldPoint(new Vector3(propX, propY));            
            this.ability.Object.transform.position = new Vector3(worldPosition.x, worldPosition.y + 3.0f, -5.0f);
            this.ability.SetFaceUp();
            this.ability.Object.gameObject.SetActive(false);
        }
    }

    public void Clear()
    {
        this.ability = null;
        this.IconImage.sprite = null;
        this.IconImage.gameObject.SetActive(false);
    }

    public void OnHoverEnter()
    {
        if (this.ability != null && this.ability.Object != null)
        {
            this.ability.Object.gameObject.SetActive(true);
        }
    }

    public void OnHoverExit()
    {
        if (this.ability != null && this.ability.Object != null)
        {
            this.ability.Object.gameObject.SetActive(false);
        }
    }

    public void OnClick()
    {
        if (this.ability != null)
        {
            Game.Player.UseAbility(this.ability);
        }
    }

    private void Awake()
    {
        this.IconImage.gameObject.SetActive(false);
        Debug.Assert(IconImage != null, "Must have IconImage set!");
    }        
}

