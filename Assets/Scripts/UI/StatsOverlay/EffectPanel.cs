using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using System;

public class EffectPanel : MonoBehaviour
{
    public EffectIcon IconTemplate;

    public EffectIcon CreateIcon(PersistentStatusEffect effect)
    {
        var data = effect.GetData();
        var icon = Instantiate(IconTemplate);
        var img = icon.GetComponent<Image>();
        if (img)
        {
            img.sprite = data.StatusIcon;
        }

        var tooltip = icon.GetComponent<UITooltipController>();
        if (tooltip)
        {
            tooltip.TextValue = data.EffectName;
        }

        icon.Identifier = effect.GetIdentifier();
        icon.transform.SetParent(transform);
        icon.transform.localScale = Vector3.one;
        return icon;
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void ExpireEffect(PersistentStatusEffect effect)
    {
        var children = transform.GetComponentsInChildren<EffectIcon>();
        var child = children.First(item => item.Identifier == effect.GetIdentifier());
        if (child)
        {
            Destroy(child.gameObject);
        }
        else
        {
            Debug.LogWarning($"No buff found with identifier {effect.GetIdentifier()}");
        }
    }
}
