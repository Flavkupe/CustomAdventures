using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public abstract class PassableTileEntity : TileEntity
{
    public virtual IEnumerator ProcessEntitySteppedOver(TileEntity other)
    {
        yield return null;
    }
}

