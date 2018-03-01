
using System;
using UnityEngine;
using UnityEngine.UI;

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
    }

    public void Clear()
    {
        this.ability = null;
        this.IconImage.sprite = null;
        this.IconImage.gameObject.SetActive(false);
    }

    public void OnClick()
    {
        if (this.ability != null)
        {
            Player.Instance.UseAbility(this.ability);
        }
    }

    private void Awake()
    {
        this.IconImage.gameObject.SetActive(false);
        Debug.Assert(IconImage != null, "Must have IconImage set!");
    }        
}

