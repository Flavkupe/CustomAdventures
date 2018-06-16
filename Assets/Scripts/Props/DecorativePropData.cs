using UnityEngine;

[CreateAssetMenu(fileName = "Prop", menuName = "Create Props/Decorative Prop", order = 1)]
public class DecorativePropData : PropData<DecorativeTileObject>
{
    public string Name;

    [AssetIcon]
    public Sprite Sprite;

    public override DecorativeTileObject InstantiateEntity()
    {
        var obj = Utils.InstantiateOfType<DecorativeTileObject>();
        obj.Data = this;
        return obj;
    }
}
