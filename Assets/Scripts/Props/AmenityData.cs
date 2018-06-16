using UnityEngine;

[CreateAssetMenu(fileName = "Prop", menuName = "Create Props/Amenity", order = 1)]
public class AmenityData : PropData<Amenity>
{
    public string Name;

    [AssetIcon]
    public Sprite Sprite;

    [Tooltip("Floaty text shown when this amenity can't be used.")]
    public string CantUseMessage = "Not now!";

    public StatusEffectData[] StatusEffects;

    public override Amenity InstantiateEntity()
    {
        var obj = Utils.InstantiateOfType<Amenity>();
        obj.Data = this;
        return obj;
    }
}

