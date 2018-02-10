using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TreasureCard : DungeonCard<TreasureCardData>
{    
    /// <summary>
    /// Prefab to spawn for this card
    /// </summary>
    public Treasure TreasureSpawn;

    public override void ExecuteTileSpawnEvent(Tile tile)
    {
        Treasure treasure = this.InstantiateTreasure();        
        DungeonManager.Instance.SpawnTreasure(treasure, tile);
    }

    private Treasure InstantiateTreasure()
    {
        Treasure treasure = Instantiate(TreasureSpawn);
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
