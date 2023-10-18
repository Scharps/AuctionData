namespace AuctionData.Application.Entities;

public abstract class Entity : IEquatable<Entity>
{
    public long Id { get; set; }

    public static bool operator ==(Entity a, Entity b)
    {
        return Equals(a, b);
    }

    public static bool operator !=(Entity a, Entity b)
    {
        return !Equals(a, b);
    }

    public bool Equals(Entity? other)
    {
        return Equals((object?)other);
    }

    public override bool Equals(object? obj)
    {
        return obj is Entity entity && Id.Equals(entity.Id);
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
}
