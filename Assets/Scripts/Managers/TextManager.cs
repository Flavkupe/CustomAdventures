using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextManager : SingletonObject<TextManager>
{
    public FloatyText DamageTextTemplate;

    // Use this for initialization
    void Awake()
    {
        Instance = this;
    }

    // Update is called once per frame
    void Update () {
		
	}
}
