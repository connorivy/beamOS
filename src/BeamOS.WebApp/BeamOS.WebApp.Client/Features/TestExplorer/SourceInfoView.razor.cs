using BeamOS.Tests.Common.Interfaces;
using Microsoft.AspNetCore.Components;

namespace BeamOS.WebApp.Client.Features.TestExplorer;

public partial class SourceInfoView
{
    [Parameter]
    public required SourceInfo SourceInfo { get; set; }
}
