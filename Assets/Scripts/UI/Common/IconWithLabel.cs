using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.State;
using TMPro;
using UnityEngine;

public class IconWithLabel : MonoBehaviour, IUpdatesWhen
{
    public TextMeshProUGUI Label;

    [Tooltip("Text including token.")]
    public string TextTemplate;

	// Use this for initialization
	void Start ()
	{
	    UpdateText();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void UpdateText()
    {
        if (Label != null && Game.Tokens != null)
        {
            Label.text = Game.Tokens.ReplaceTokens(TextTemplate);
        }
    }

    [Tooltip("Optional condition which can cause text to update.")]
    public SimpleWorldEvent Condition;
    public SimpleWorldEvent RequiredCondition => Condition;
    public void Updated()
    {
        UpdateText();
    }
}
