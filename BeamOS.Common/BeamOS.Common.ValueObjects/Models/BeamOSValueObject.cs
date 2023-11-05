namespace BeamOS.Common.Domain.Models;

public abstract class BeamOSValueObject : IEquatable<BeamOSValueObject>
{
    protected static bool EqualOperator(BeamOSValueObject left, BeamOSValueObject right)
    {
        if (left is null ^ right is null)
        {
            return false;
        }
        return ReferenceEquals(left, right) || left.Equals(right);
    }

    protected static bool NotEqualOperator(BeamOSValueObject left, BeamOSValueObject right)
    {
        return !EqualOperator(left, right);
    }

    protected abstract IEnumerable<object> GetEqualityComponents();

    public override bool Equals(object? obj)
    {
        if (obj == null || obj.GetType() != this.GetType())
        {
            return false;
        }

        var other = (BeamOSValueObject)obj;

        return this.GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
    }

    public override int GetHashCode()
    {
        return this.GetEqualityComponents()
            .Select(x => x != null ? x.GetHashCode() : 0)
            .Aggregate((x, y) => x ^ y);
    }

    public bool Equals(BeamOSValueObject? other)
    {
        return this.Equals((object?)other);
    }
}
