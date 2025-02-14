using System.Text.Json.Serialization;

namespace BeamOs.Common.Contracts;

//public readonly record struct BeamOsError
public sealed class BeamOsError
{
    [JsonConstructor]
    [Obsolete("JSON ctor. Don't use")]
    public BeamOsError(
        string code,
        string description,
        ErrorType type,
        Dictionary<string, string>? metadata
    )
    {
        this.Code = code;
        this.Description = description;
        this.Type = type;
        this.NumericType = (int)type;
        this.Metadata = metadata;
    }

    /// <summary>
    /// Gets the unique error code.
    /// </summary>
    public string Code { get; }

    /// <summary>
    /// Gets the error description.
    /// </summary>
    public string Description { get; }

    /// <summary>
    /// Gets the error type.
    /// </summary>
    public ErrorType Type { get; }

    /// <summary>
    /// Gets the numeric value of the type.
    /// </summary>
    public int NumericType { get; }

    /// <summary>
    /// Gets the metadata.
    /// </summary>
    public Dictionary<string, string>? Metadata { get; }

    /// <summary>
    /// Creates an <see cref="BeamOsError"/> of type <see cref="ErrorType.Failure"/> from a code and description.
    /// </summary>
    /// <param name="code">The unique error code.</param>
    /// <param name="description">The error description.</param>
    /// <param name="metadata">A dictionary which provides optional space for information.</param>
    public static BeamOsError Failure(
        string code = "General.Failure",
        string description = "A failure has occurred.",
        Dictionary<string, string>? metadata = null
    ) => new(code, description, ErrorType.Failure, metadata);

    /// <summary>
    /// Creates an <see cref="BeamOsError"/> of type <see cref="ErrorType.Unexpected"/> from a code and description.
    /// </summary>
    /// <param name="code">The unique error code.</param>
    /// <param name="description">The error description.</param>
    /// <param name="metadata">A dictionary which provides optional space for information.</param>
    public static BeamOsError Unexpected(
        string code = "General.Unexpected",
        string description = "An unexpected error has occurred.",
        Dictionary<string, string>? metadata = null
    ) => new(code, description, ErrorType.Unexpected, metadata);

    /// <summary>
    /// Creates an <see cref="BeamOsError"/> of type <see cref="ErrorType.Validation"/> from a code and description.
    /// </summary>
    /// <param name="code">The unique error code.</param>
    /// <param name="description">The error description.</param>
    /// <param name="metadata">A dictionary which provides optional space for information.</param>
    public static BeamOsError Validation(
        string code = "General.Validation",
        string description = "A validation error has occurred.",
        Dictionary<string, string>? metadata = null
    ) => new(code, description, ErrorType.Validation, metadata);

    /// <summary>
    /// Creates an <see cref="BeamOsError"/> of type <see cref="ErrorType.Conflict"/> from a code and description.
    /// </summary>
    /// <param name="code">The unique error code.</param>
    /// <param name="description">The error description.</param>
    /// <param name="metadata">A dictionary which provides optional space for information.</param>
    public static BeamOsError Conflict(
        string code = "General.Conflict",
        string description = "A conflict error has occurred.",
        Dictionary<string, string>? metadata = null
    ) => new(code, description, ErrorType.Conflict, metadata);

    /// <summary>
    /// Creates an <see cref="BeamOsError"/> of type <see cref="ErrorType.NotFound"/> from a code and description.
    /// </summary>
    /// <param name="code">The unique error code.</param>
    /// <param name="description">The error description.</param>
    /// <param name="metadata">A dictionary which provides optional space for information.</param>
    public static BeamOsError NotFound(
        string code = "General.NotFound",
        string description = "A 'Not Found' error has occurred.",
        Dictionary<string, string>? metadata = null
    ) => new(code, description, ErrorType.NotFound, metadata);

    /// <summary>
    /// Creates an <see cref="BeamOsError"/> of type <see cref="ErrorType.Unauthorized"/> from a code and description.
    /// </summary>
    /// <param name="code">The unique error code.</param>
    /// <param name="description">The error description.</param>
    /// <param name="metadata">A dictionary which provides optional space for information.</param>
    public static BeamOsError Unauthorized(
        string code = "General.Unauthorized",
        string description = "An 'Unauthorized' error has occurred.",
        Dictionary<string, string>? metadata = null
    ) => new(code, description, ErrorType.Unauthorized, metadata);

    /// <summary>
    /// Creates an <see cref="BeamOsError"/> of type <see cref="ErrorType.Forbidden"/> from a code and description.
    /// </summary>
    /// <param name="code">The unique error code.</param>
    /// <param name="description">The error description.</param>
    /// <param name="metadata">A dictionary which provides optional space for information.</param>
    public static BeamOsError Forbidden(
        string code = "General.Forbidden",
        string description = "A 'Forbidden' error has occurred.",
        Dictionary<string, string>? metadata = null
    ) => new(code, description, ErrorType.Forbidden, metadata);

    /// <summary>
    /// Creates an <see cref="BeamOsError"/> with the given numeric <paramref name="type"/>,
    /// <paramref name="code"/>, and <paramref name="description"/>.
    /// </summary>
    /// <param name="type">An integer value which represents the type of error that occurred.</param>
    /// <param name="code">The unique error code.</param>
    /// <param name="description">The error description.</param>
    /// <param name="metadata">A dictionary which provides optional space for information.</param>
    public static BeamOsError Custom(
        int type,
        string code,
        string description,
        Dictionary<string, string>? metadata = null
    ) => new(code, description, (ErrorType)type, metadata);

    public bool Equals(BeamOsError other)
    {
        if (
            this.Type != other.Type
            || this.NumericType != other.NumericType
            || this.Code != other.Code
            || this.Description != other.Description
        )
        {
            return false;
        }

        if (this.Metadata is null)
        {
            return other.Metadata is null;
        }

        return other.Metadata is not null && CompareMetadata(this.Metadata, other.Metadata);
    }

    public override int GetHashCode() =>
        this.Metadata is null
            ? HashCode.Combine(this.Code, this.Description, this.Type, this.NumericType)
            : this.ComposeHashCode();

    private int ComposeHashCode()
    {
#pragma warning disable SA1129 // HashCode needs to be instantiated this way
        var hashCode = new HashCode();
#pragma warning restore SA1129

        hashCode.Add(this.Code);
        hashCode.Add(this.Description);
        hashCode.Add(this.Type);
        hashCode.Add(this.NumericType);

        foreach (var keyValuePair in this.Metadata!)
        {
            hashCode.Add(keyValuePair.Key);
            hashCode.Add(keyValuePair.Value);
        }

        return hashCode.ToHashCode();
    }

    private static bool CompareMetadata(
        Dictionary<string, string> metadata,
        Dictionary<string, string> otherMetadata
    )
    {
        if (ReferenceEquals(metadata, otherMetadata))
        {
            return true;
        }

        if (metadata.Count != otherMetadata.Count)
        {
            return false;
        }

        foreach (var keyValuePair in metadata)
        {
            if (
                !otherMetadata.TryGetValue(keyValuePair.Key, out var otherValue)
                || !keyValuePair.Value.Equals(otherValue)
            )
            {
                return false;
            }
        }

        return true;
    }
}
