using UnityEngine;

[CreateAssetMenu(fileName = "Prop", menuName = "Create Props/Amenity", order = 1)]
public class AmenityData : PropData<Amenity>
{
    public string Name;
    public Sprite Sprite;

    public override Amenity InstantiateEntity()
    {
        var obj = Utils.InstantiateOfType<Amenity>();
        obj.Data = this;
        return obj;
    }
}
