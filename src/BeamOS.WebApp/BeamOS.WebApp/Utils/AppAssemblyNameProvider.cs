using BeamOS.WebApp.Components;

namespace BeamOS.WebApp.Utils;

public class AppAssemblyNameProvider : IAppAssemblyNameProvider
{
    public string AssemblyName => typeof(App).Assembly.GetName().Name ?? throw new Exception("TODO");
}

public interface IAppAssemblyNameProvider
{
    public string AssemblyName { get; }
}

