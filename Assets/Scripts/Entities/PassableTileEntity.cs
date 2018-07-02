using System.Collections;

public abstract class PassableTileEntity : TileEntity
{
    public virtual IEnumerator ProcessEntitySteppedOver(TileEntity other)
    {
        yield return null;
    }
}

