namespace BeamOs.Common.Domain.Interfaces;

public interface IConstructable<TSelf, T1>
{
    public static abstract TSelf Construct(T1 t1);
}

public interface IConstructable<TSelf, T1, T2>
{
    static abstract TSelf Construct(T1 t1, T2 t2);
}
