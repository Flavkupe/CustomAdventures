using Assets.Scripts;
using System;
using System.ComponentModel;

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
    public IntObservable HP = new IntObservable(10);
    public int Energy = 0;
    public int Strength = 1;
    public IntObservable FullActions = new IntObservable(1);
    public IntObservable FreeMoves = new IntObservable(1);
    public int VisibilityRange = 3;

    public Stats Clone()
    {
        var newStats = MemberwiseClone() as Stats;
        newStats.HP = new IntObservable(HP);
        newStats.FullActions = new IntObservable(FullActions);
        newStats.FreeMoves = new IntObservable(FreeMoves);
        return newStats;
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