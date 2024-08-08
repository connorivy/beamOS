using BeamOS.Tests.Common.Interfaces;
using Microsoft.AspNetCore.Components;

namespace BeamOs.WebApp.Client.Components.Features.TestExplorer;

public partial class SourceInfoView
{
    [Parameter]
    public required SourceInfo SourceInfo { get; set; }
}
