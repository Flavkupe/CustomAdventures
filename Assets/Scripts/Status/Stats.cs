using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Stats
{
    public int Level = 1;
    public int HP = 10;
    public int Energy = 0;
    public int Strength = 1;
    public int FullActions = 1;
    public int FreeMoves = 1;
    public int VisibilityRange = 3;

    public Stats Clone()
    {
        return MemberwiseClone() as Stats;
    }

    /// <summary>
    /// Adds the contents of stats to this stats
    /// </summary>
    /// <param name="stats"></param>
    public void Accumulate(Stats stats)
    {
        Level += stats.Level;
        HP += stats.HP;
        Energy += stats.Energy;
        Strength += stats.Strength;
        FullActions += stats.FullActions;
        FreeMoves += stats.FreeMoves;
        VisibilityRange += stats.VisibilityRange;
    }
}