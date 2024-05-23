using BeamOS.WebApp.EditorApi;

namespace BeamOS.Tests.Common.Interfaces;

public interface ITestFixtureDisplayable
{
    public Task Display(IEditorApiAlpha editorApiAlpha);
}
