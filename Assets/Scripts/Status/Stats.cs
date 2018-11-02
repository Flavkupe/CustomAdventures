using Assets.Scripts;
using System;
using System.ComponentModel;
using System.Diagnostics;

[Serializable]
public class Stats
{
    public Stats()
    {
        HP.PropertyChanged += OnPropertyChanged;
        FullActions.PropertyChanged += OnPropertyChanged;
        FreeMoves.PropertyChanged += OnPropertyChanged;
    }

    protected virtual void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
    }

    public int Level = 1;
    public readonly IntObservable HP = new IntObservable(10);
    public int Energy = 0;
    public int Strength = 1;
    public readonly IntObservable FullActions = new IntObservable(1);
    public readonly IntObservable FreeMoves = new IntObservable(1);
    public int VisibilityRange = 3;

    public Stats Clone()
    {
        var newStats = new Stats
        {
            Level = Level,
            Energy = Energy,
            VisibilityRange = VisibilityRange,
        };

        newStats.HP.Set(HP);
        newStats.FullActions.Set(FullActions);
        newStats.FreeMoves.Set(FreeMoves);
        return newStats;
    }

    /// <summary>
    /// Adds the contents of stats to this stats
    /// </summary>
    /// <param name="stats"></param>
    public void Accumulate(Stats stats)
    {
        Level += stats.Level;
        HP.Value += stats.HP;
        Energy += stats.Energy;
        Strength += stats.Strength;
        FullActions.Value += stats.FullActions;
        FreeMoves.Value += stats.FreeMoves;
        VisibilityRange += stats.VisibilityRange;
    }
}