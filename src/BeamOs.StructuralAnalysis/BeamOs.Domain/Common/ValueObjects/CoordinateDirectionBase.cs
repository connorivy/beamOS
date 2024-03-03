using System.Collections;
using BeamOs.Domain.Common.Enums;
using BeamOs.Domain.Common.Models;

namespace BeamOs.Domain.Common.ValueObjects;

public abstract class CoordinateDirectionBase<T> : CoordinateDirectionBase<T, T>, IEnumerable<T>
    where T : notnull
{
    protected CoordinateDirectionBase(
        T valueAlongX,
        T valueAlongY,
        T valueAlongZ,
        T valueAboutX,
        T valueAboutY,
        T valueAboutZ
    )
        : base(valueAlongX, valueAlongY, valueAlongZ, valueAboutX, valueAboutY, valueAboutZ) { }

    public IEnumerator<T> GetEnumerator()
    {
        yield return this.AlongX;
        yield return this.AlongY;
        yield return this.AlongZ;
        yield return this.AboutX;
        yield return this.AboutY;
        yield return this.AboutZ;
    }

    public T GetValueInDirection(CoordinateSystemDirection3D direction)
    {
        return direction switch
        {
            CoordinateSystemDirection3D.AlongX => this.AlongX,
            CoordinateSystemDirection3D.AlongY => this.AlongY,
            CoordinateSystemDirection3D.AlongZ => this.AlongZ,
            CoordinateSystemDirection3D.AboutX => this.AboutX,
            CoordinateSystemDirection3D.AboutY => this.AboutY,
            CoordinateSystemDirection3D.AboutZ => this.AboutZ,
            CoordinateSystemDirection3D.Undefined => throw new NotImplementedException(),
            _ => throw new NotImplementedException(),
        };
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return this.AlongX;
        yield return this.AlongY;
        yield return this.AlongZ;
        yield return this.AboutX;
        yield return this.AboutY;
        yield return this.AboutZ;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return this.GetEnumerator();
    }
}

public abstract class CoordinateDirectionBase<TLinear, TRotational> : BeamOSValueObject
    where TLinear : notnull
    where TRotational : notnull
{
    private readonly TLinear[] linearValues = new TLinear[3];
    private readonly TRotational[] rotationalValues = new TRotational[3];

    protected CoordinateDirectionBase(
        TLinear valueAlongX,
        TLinear valueAlongY,
        TLinear valueAlongZ,
        TRotational valueAboutX,
        TRotational valueAboutY,
        TRotational valueAboutZ
    )
    {
        this.linearValues[0] = valueAlongX;
        this.linearValues[1] = valueAlongY;
        this.linearValues[2] = valueAlongZ;
        this.rotationalValues[0] = valueAboutX;
        this.rotationalValues[1] = valueAboutY;
        this.rotationalValues[2] = valueAboutZ;
    }

    /// <summary>
    /// Value along the x axis
    /// </summary>
    protected TLinear AlongX
    {
        get => this.linearValues[0];
        set => this.linearValues[0] = value;
    }

    /// <summary>
    /// Value along the y axis
    /// </summary>
    protected TLinear AlongY
    {
        get => this.linearValues[1];
        set => this.linearValues[1] = value;
    }

    /// <summary>
    /// Value along the z axis
    /// </summary>
    protected TLinear AlongZ
    {
        get => this.linearValues[2];
        set => this.linearValues[2] = value;
    }

    /// <summary>
    /// Value about the x axis
    /// </summary>
    protected TRotational AboutX
    {
        get => this.rotationalValues[0];
        set => this.rotationalValues[0] = value;
    }

    /// <summary>
    /// Value about the y axis
    /// </summary>
    protected TRotational AboutY
    {
        get => this.rotationalValues[1];
        set => this.rotationalValues[1] = value;
    }

    /// <summary>
    /// Value about the z axis
    /// </summary>
    protected TRotational AboutZ
    {
        get => this.rotationalValues[2];
        set => this.rotationalValues[2] = value;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return this.AlongX;
        yield return this.AlongY;
        yield return this.AlongZ;
        yield return this.AboutX;
        yield return this.AboutY;
        yield return this.AboutZ;
    }
}
