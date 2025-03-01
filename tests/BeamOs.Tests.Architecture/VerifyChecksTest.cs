using VerifyTUnit;

namespace BeamOs.Tests.Architecture;

public class VerifyChecksTest
{
    [Test]
    public Task RunVerifyChecks() => VerifyChecks.Run();
}

public class Test
{
    //private static readonly ArchUnitNET.Domain.Architecture Architecture =
    //new ArchLoader().LoadAssemblies(
    //    System.Reflection.Assembly.Load("ExampleClassAssemblyName"),
    //    System.Reflection.Assembly.Load("ForbiddenClassAssemblyName")
    //).Build();

    //private readonly IObjectProvider<IType> ExampleLayer =
    //Types().That().ResideInAssembly("ExampleAssembly").As("Example Layer");

    //private readonly IObjectProvider<Class> ExampleClasses =
    //    Classes().That().ImplementInterface("IExampleInterface").As("Example Classes");

    //private readonly IObjectProvider<IType> ForbiddenLayer =
    //    Types().That().ResideInNamespace("ForbiddenNamespace").As("Forbidden Layer");

    //private readonly IObjectProvider<Interface> ForbiddenInterfaces =
    //    Interfaces().That().HaveFullNameContaining("forbidden").As("Forbidden Interfaces");

    //[Test]
    //public void Test()
    //{
    //    Types().That().ResideInAssembly(typeof(IAssemblyMarkerStructuralAnalysisApplication).Assembly).Should()
    //}
}
