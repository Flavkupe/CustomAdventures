using System;

public interface IStackable
{
    int StackSize { get; set; }
    int MaxStack { get; }
    int SpaceLeft { get; }
    string Identifier { get; }

    bool ItemCanStack(IStackable other);
    void StackItems(IStackable other);
}

public abstract class StackableItem : IStackable
{
    public int CurrentStackSize = 1;

    public virtual int StackSize
    {
        get { return CurrentStackSize; }
        set { CurrentStackSize = value; }
    }

    public abstract int MaxStack { get; } 

    public virtual int SpaceLeft { get { return MaxStack - StackSize; } }

    public abstract string Identifier { get; }

    public virtual bool ItemCanStack(IStackable other)
    {
        return other.Identifier == Identifier && SpaceLeft > 0;
    }

    /// <summary>
    /// Adds other item to stack, changing CurrentStackSize for each
    /// </summary>
    public virtual void StackItems(IStackable other)
    {
        int newItems = Math.Min(SpaceLeft, other.StackSize);
        int leftOver = other.StackSize - newItems;
        CurrentStackSize += newItems;
        other.StackSize = leftOver;
    }
}

