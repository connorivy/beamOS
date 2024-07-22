using System.Text.Json.Serialization;

namespace BeamOs.Common.Api;

public class Result
{
    [JsonConstructor]
    private Result(bool isSuccess, BeamOsError error)
    {
        if (isSuccess && error != BeamOsError.None || !isSuccess && error == BeamOsError.None)
        {
            throw new ArgumentException("Invalid error", nameof(error));
        }

        this.IsSuccess = isSuccess;
        this.Error = error;
    }

    public bool IsSuccess { get; }

    [JsonIgnore]
    public bool IsFailure => !this.IsSuccess;

    public BeamOsError Error { get; }

    public static Result Success() => new(true, BeamOsError.None);

    public static Result Failure(BeamOsError error) => new(false, error);
}
