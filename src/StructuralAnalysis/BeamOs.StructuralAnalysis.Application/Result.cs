//namespace BeamOs.StructuralAnalysis.Application;

//public readonly struct Result<TValue>
//{
//    public TValue? Value { get; }

//    public Exception? Error { get; }

//    private Result(TValue value)
//    {
//        this.isError = false;
//        this.Value = value;
//    }

//    private Result(Exception error)
//    {
//        this.isError = true;
//        this.Error = error;
//    }

//    private readonly bool isError;
//    private bool IsSuccessField => !this.isError;

//    public static implicit operator Result<TValue>(TValue value) => new(value);

//    public static implicit operator Result<TValue>(Exception error) => new(error);

//    public bool IsError(out Exception? error)
//    {
//        if (!this.isError)
//        {
//            error = default;
//            return false;
//        }

//        error = this.Error!;
//        return true;
//    }

//    public bool IsSuccess(out TValue? value)
//    {
//        if (!this.IsSuccessField)
//        {
//            value = default;
//            return false;
//        }

//        value = this.Value!;
//        return true;
//    }

//    public TResult Match<TResult>(
//        Func<TValue, TResult> success,
//        Func<Exception, TResult> failure
//    ) => !this.isError ? success(this.Value!) : failure(this.Error!);

//    public async Task<TResult> MatchAsync<TResult>(
//        Func<TValue, TResult> success,
//        Func<Exception, TResult> failure
//    ) => !this.isError ? success(this.Value!) : failure(this.Error!);
//}
