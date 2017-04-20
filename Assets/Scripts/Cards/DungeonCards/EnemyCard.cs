using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCard : DungeonCard
{
    public EnemyCardData Data;

    /// <summary>
    /// Prefab to spawn for this card
    /// </summary>
    public Enemy EnemySpawn;

    public override DungeonCardType DungeonCardType { get { return DungeonCardType.Enemy; } }

    public Enemy InstantiateEnemy()
    {
        Enemy enemy = Instantiate(EnemySpawn);
        enemy.Data = this.Data.Clone();
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

[Serializable]
public class EnemyCardData
{
    public string Name;
    public int Level = 1;
    public int HP = 1;
    public int Movement = 1;
    public int Attack = 1;
    public int EXP = 1;

    public EnemyCardData Clone()
    {
        return (EnemyCardData)this.MemberwiseClone();
    }
}