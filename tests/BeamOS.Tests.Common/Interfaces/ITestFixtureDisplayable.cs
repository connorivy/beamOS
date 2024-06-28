using BeamOs.CodeGen.Apis.EditorApi;

namespace BeamOS.Tests.Common.Interfaces;

public interface ITestFixtureDisplayable : IHasSourceInfo
{
    public Task Display(IEditorApiAlpha editorApiAlpha);
}
