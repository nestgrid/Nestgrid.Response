namespace Nestgrid.Response;

/// <summary>
/// Represents an operation outcome that may include a value.
/// </summary>
/// <typeparam name="T">The value type.</typeparam>
public sealed class Result<T> : Result
{
    internal Result(
        ResultStatus status,
        T? value = default,
        params ResultMessage[] messages)
        : base(status, messages)
    {
        Value = value;
    }

    /// <summary>
    /// Gets the result value.
    /// </summary>
    public T? Value { get; }
}
