
public class Amenity : TileEntity
{
    public AmenityData Data { get; set; }

    public override TileEntityType EntityType => TileEntityType.Amenity;
}

