using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EnemyCard : DungeonCard<EnemyCardData>
{
    // TODO: have a generic TileEntity gameobject, create it, and apply all the Components rather than
    // having to create one for each type
    /// <summary>
    /// Prefab to spawn for this card
    /// </summary>
    public Enemy EnemySpawn;

    public override DungeonCardType DungeonCardType { get { return DungeonCardType.Enemy; } }

    public override void ExecuteTileSpawnEvent(Tile tile)
    {
        Enemy enemy = this.InstantiateEnemy();        
        DungeonManager.Instance.SpawnEnemy(enemy, tile);
    }

    public Enemy InstantiateEnemy()
    {
        Enemy enemy = Instantiate(EnemySpawn);
        enemy.Data = this.Data;
        return enemy;
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
