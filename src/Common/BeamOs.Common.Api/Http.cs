namespace BeamOs.Common.Api;

public static class Http
{
    /// <summary>
    /// retrieve a record
    /// </summary>
    public const string Get = nameof(Get);

    /// <summary>
    /// create a record
    /// </summary>
    public const string Post = nameof(Post);

    /// <summary>
    /// replace a record
    /// </summary>
    public const string Put = nameof(Put);

    /// <summary>
    /// partially update a record
    /// </summary>
    public const string Patch = nameof(Patch);

    /// <summary>
    /// remove a record
    /// </summary>
    public const string Delete = nameof(Delete);

    /// <summary>
    /// retrieve only headers
    /// </summary>
    public const string Head = nameof(Head);

    /// <summary>
    /// retrieve communication options
    /// </summary>
    public const string Options = nameof(Options);
}
