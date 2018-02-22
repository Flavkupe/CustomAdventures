﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TreasureCard : DungeonCard<TreasureCardData>
{
    public override void ExecuteTileSpawnEvent(Tile tile)
    {
        Treasure treasure = this.InstantiateTreasure();        
        DungeonManager.Instance.SpawnTreasure(treasure, tile);
    }

    private Treasure InstantiateTreasure()
    {
        Treasure treasure = InstantiateOfType<Treasure>();
        treasure.Data = this.Data;
        return treasure;
    }

    // Use this for initialization
    void Start ()
    {
	}
	
	// Update is called once per frame
	void Update ()
    {
	}
}