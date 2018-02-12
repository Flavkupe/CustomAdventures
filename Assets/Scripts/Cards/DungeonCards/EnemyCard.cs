using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EnemyCard : DungeonCard<EnemyCardData>
{
    public override void ExecuteTileSpawnEvent(Tile tile)
    {
        Enemy enemy = this.InstantiateEnemy();        
        DungeonManager.Instance.SpawnEnemy(enemy, tile);
    }

    public Enemy InstantiateEnemy()
    {
        Enemy enemy = InstantiateOfType<Enemy>();
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
