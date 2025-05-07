using System.Diagnostics.CodeAnalysis;
using BeamOs.Application.Common.Mappers.UnitValueDtoMappers;
using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.MomentLoads;
using BeamOs.StructuralAnalysis.Contracts.Common;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.LoadCases;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.MomentLoad;
using BeamOs.WebApp.Components.Features.Editor;
using BeamOs.WebApp.Components.Features.SelectionInfo;
using BeamOs.WebApp.EditorCommands;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;

namespace BeamOs.WebApp.Components.Features.ModelObjectEditor.MomentLoads;

public partial class MomentLoadObjectEditor(
    IDispatcher dispatcher,
    IState<EditorComponentState> editorState,
    IState<MomentLoadObjectEditorState> state,
    PutMomentLoadSimpleCommandHandler putMomentLoadCommandHandler,
    CreateMomentLoadClientCommandHandler createMomentLoadCommandHandler,
    DeleteMomentLoadSimpleCommandHandler deleteMomentLoadCommandHandler
) : FluxorComponent
{
    private readonly MomentLoadModel momentLoad = new();

    private int MomentLoadId
    {
        get => this.momentLoad.Id;
        set
        {
            if (value > 0)
            {
                this.UpdateFromMomentLoadId(value);
            }
            this.momentLoad.Id = value;
        }
    }

    private LoadCase? LoadCase
    {
        get =>
            editorState.Value.CachedModelResponse.LoadCases.GetValueOrDefault(
                this.momentLoad.LoadCaseId
            );
        set => this.momentLoad.LoadCaseId = value.Id;
    }
    private const int ResultLimit = 50;

    private string MomentLoadIdText =>
        this.MomentLoadId == 0 ? "New Moment Load" : this.MomentLoadId.ToString();

    [Parameter]
    public required UnitSettingsContract UnitSettings { get; set; }

    [Parameter]
    public required Guid ModelId { get; init; }

    [Parameter]
    public SelectedObject? SelectedObject { get; set; }

    protected override void OnInitialized()
    {
        // putMomentLoadCommandHandler.IsLoadingChanged += this.PutMomentLoadCommandHandler_IsLoadingChanged;
        // createMomentLoadCommandHandler.IsLoadingChanged += this.PutMomentLoadCommandHandler_IsLoadingChanged;
        // deleteMomentLoadCommandHandler.IsLoadingChanged += this.PutMomentLoadCommandHandler_IsLoadingChanged;

        base.OnInitialized();
        this.SubscribeToAction<PutMomentLoadClientCommand>(command =>
        {
            if (command.New is null)
            {
                return;
            }

            if (command.New.Id == this.SelectedObject?.Id)
            {
                this.UpdateFromMomentLoadResponse(command.New);
            }
        });
    }

    // private void PutMomentLoadCommandHandler_IsLoadingChanged(object? sender, bool e) =>
    //     dispatcher.Dispatch(new MomentLoadObjectEditorIsLoading(e));

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        if (this.SelectedObject is not null && this.SelectedObject.TypeName == "MomentLoad")
        {
            this.UpdateFromMomentLoadId(this.SelectedObject.Id);
        }
    }

    private void UpdateFromMomentLoadId(int nodeId)
    {
        var response = editorState.Value.CachedModelResponse.MomentLoads[nodeId];
        this.UpdateFromMomentLoadResponse(response);
    }

    private void UpdateFromMomentLoadResponse(MomentLoadResponse response)
    {
        this.momentLoad.Id = response.Id;
        this.momentLoad.ModelId = response.ModelId;
        this.momentLoad.NodeId = response.NodeId;
        this.momentLoad.LoadCaseId = response.LoadCaseId;
        this.momentLoad.Torque = response
            .Torque.MapToTorque()
            .As(this.UnitSettings.TorqueUnit.MapToTorqueUnit());
        this.momentLoad.AxisDirection = new(
            response.AxisDirection.X,
            response.AxisDirection.Y,
            response.AxisDirection.Z
        );
    }

    private void OnFocus(string fieldName, int fieldNum, Action<object> setValue)
    {
        dispatcher.Dispatch(new FieldSelected(new(fieldName, fieldNum, setValue)));
    }

    private async Task Delete()
    {
        ModelEntityCommand command = new() { ModelId = this.ModelId, Id = this.momentLoad.Id };
        await deleteMomentLoadCommandHandler.ExecuteAsync(command);
    }

    private async Task Submit()
    {
        MomentLoadData nodeData = new()
        {
            NodeId = this.momentLoad.NodeId,
            LoadCaseId = this.momentLoad.Id,
            Torque = new(this.momentLoad.Torque.Value, this.UnitSettings.TorqueUnit),
            AxisDirection = new(
                this.momentLoad.AxisDirection.X.Value,
                this.momentLoad.AxisDirection.Y.Value,
                this.momentLoad.AxisDirection.Z.Value
            ),
        };

        if (this.momentLoad.Id == 0)
        {
            CreateMomentLoadClientCommand command = new(nodeData) { ModelId = this.ModelId };
            await createMomentLoadCommandHandler.ExecuteAsync(command);
        }
        else
        {
            PutMomentLoadCommand command = new()
            {
                Id = this.momentLoad.Id,
                ModelId = this.ModelId,
                Body = nodeData,
            };

            await putMomentLoadCommandHandler.ExecuteAsync(command);
        }
    }

    private static readonly int[] NullInt = [0];

    private Task<IEnumerable<int>> MomentLoadIds(string searchText, CancellationToken ct)
    {
        if (!int.TryParse(searchText, out int subInt))
        {
            return Task.FromResult(
                NullInt.Concat(editorState.Value.CachedModelResponse.MomentLoads.Keys)
            );
        }

        // Get the number of digits in the subInt
        int subIntLength = GetNumberOfDigits(subInt);

        return Task.FromResult(
            NullInt.Concat(
                editorState.Value.CachedModelResponse.MomentLoads.Keys.Where(k =>
                    GetPrefix(k, subIntLength) == subInt
                )
            )
        );
    }

    private Task<IEnumerable<int>> NodeIds(string searchText, CancellationToken ct)
    {
        return GetPossibleIdsFromSubstring(
            searchText,
            editorState.Value.CachedModelResponse.Nodes.Keys,
            false
        );
    }

    private static Task<IEnumerable<int>> GetPossibleIdsFromSubstring(
        string searchText,
        IEnumerable<int> keys,
        bool includeNew = false
    )
    {
        IEnumerable<int> result;
        if (includeNew)
        {
            result = NullInt.Concat(GetPossibleIdsFromSubstring(searchText, keys));
        }
        else
        {
            result = GetPossibleIdsFromSubstring(searchText, keys);
        }

        return Task.FromResult(result);
    }

    private static IEnumerable<int> GetPossibleIdsFromSubstring(
        string searchText,
        IEnumerable<int> keys
    )
    {
        if (!int.TryParse(searchText, out int subInt) || subInt <= 0)
        {
            return keys;
        }

        // Get the number of digits in the subInt
        int subIntLength = GetNumberOfDigits(subInt);

        return keys.Where(k => GetPrefix(k, subIntLength) == subInt);
    }

    // Helper method to get the number of digits in an integer
    private static int GetNumberOfDigits(int number)
    {
        if (number == 0)
            return 1; // Edge case for 0
        return (int)Math.Floor(Math.Log10(Math.Abs(number))) + 1;
    }

    // Helper method to get the prefix of a number with a specified length
    private static int GetPrefix(int number, int prefixLength)
    {
        int numberOfDigits = GetNumberOfDigits(number);
        if (prefixLength >= numberOfDigits)
        {
            return number; // The entire number is the prefix
        }
        return number / (int)Math.Pow(10, numberOfDigits - prefixLength);
    }

    private Task<IEnumerable<LoadCase>> LoadCases(string searchText, CancellationToken ct)
    {
        IEnumerable<LoadCase> result = editorState.Value.CachedModelResponse.LoadCases.Values;
        if (!string.IsNullOrWhiteSpace(searchText))
        {
            result = result.Where(lc =>
                lc.Name.Contains(searchText, StringComparison.OrdinalIgnoreCase)
            );
        }
        return Task.FromResult(result.OrderBy(lc => lc.Name).AsEnumerable());
    }

    // protected override ValueTask DisposeAsyncCore(bool disposing)
    // {
    //     putMomentLoadCommandHandler.IsLoadingChanged -= this.PutMomentLoadCommandHandler_IsLoadingChanged;
    //     createMomentLoadCommandHandler.IsLoadingChanged -= this.PutMomentLoadCommandHandler_IsLoadingChanged;
    //     deleteMomentLoadCommandHandler.IsLoadingChanged -= this.PutMomentLoadCommandHandler_IsLoadingChanged;
    //     return base.DisposeAsyncCore(disposing);
    // }

    public class MomentLoadModel
    {
        public int Id { get; set; }
        public Guid ModelId { get; set; }
        public int NodeId { get; set; }
        public int LoadCaseId { get; set; }
        public double? Torque { get; set; }
        public Vector3 AxisDirection { get; set; } = new(0, 0, 1);
    }

    public record Vector3
    {
        public required double? X { get; set; }
        public required double? Y { get; set; }
        public required double? Z { get; set; }

        public Vector3() { }

        [SetsRequiredMembers]
        public Vector3(double x, double y, double z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }
    }
}
