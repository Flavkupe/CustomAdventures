using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DungeonCard : Card
{
    public override CardType CardType { get { return CardType.Dungeon; } }

    public abstract DungeonCardType DungeonCardType { get; }

    // Use this for initialization
    void Start ()
    {		
	}
	
	// Update is called once per frame
	void Update ()
    {		
	}
}

public enum DungeonCardType
{
    Enemy,
    Treasure,
    Trap,
}
