using System.ComponentModel.DataAnnotations;

namespace Nestgrid.Response.Extensions.Validation;

/// <summary>
/// Provides extensions for converting data annotations validation results.
/// </summary>
public static class ValidationResultExtensions
{
    /// <summary>
    /// Converts a validation result to a result message.
    /// </summary>
    /// <param name="validationResult">The validation result to convert.</param>
    /// <param name="severity">The message severity.</param>
    /// <returns>A result message.</returns>
    public static ResultMessage ToMessage(
        this ValidationResult validationResult,
        ResultMessageSeverity severity = ResultMessageSeverity.Warning)
    {
        if (validationResult is null)
        {
            throw new ArgumentNullException(nameof(validationResult));
        }

        return CreateMessage(validationResult.ErrorMessage, severity);
    }

    /// <summary>
    /// Converts validation results to result messages.
    /// </summary>
    /// <param name="validationResults">The validation results to convert.</param>
    /// <param name="severity">The message severity.</param>
    /// <returns>The converted result messages.</returns>
    public static ResultMessage[] ToMessages(
        this IEnumerable<ValidationResult> validationResults,
        ResultMessageSeverity severity = ResultMessageSeverity.Warning)
    {
        if (validationResults is null)
        {
            throw new ArgumentNullException(nameof(validationResults));
        }

        return validationResults
            .Select(validationResult => validationResult.ToMessage(severity))
            .ToArray();
    }

    /// <summary>
    /// Converts a validation result to an invalid result.
    /// </summary>
    /// <param name="validationResult">The validation result to convert.</param>
    /// <param name="severity">The message severity.</param>
    /// <returns>An invalid result.</returns>
    public static Result ToInvalidResult(
        this ValidationResult validationResult,
        ResultMessageSeverity severity = ResultMessageSeverity.Warning) =>
        new[] { validationResult }.ToInvalidResult(severity);

    /// <summary>
    /// Converts a validation result to a typed invalid result.
    /// </summary>
    /// <typeparam name="T">The result value type.</typeparam>
    /// <param name="validationResult">The validation result to convert.</param>
    /// <param name="severity">The message severity.</param>
    /// <returns>A typed invalid result.</returns>
    public static Result<T> ToInvalidResult<T>(
        this ValidationResult validationResult,
        ResultMessageSeverity severity = ResultMessageSeverity.Warning) =>
        new[] { validationResult }.ToInvalidResult<T>(severity);

    /// <summary>
    /// Converts validation results to an invalid result.
    /// </summary>
    /// <param name="validationResults">The validation results to convert.</param>
    /// <param name="severity">The message severity.</param>
    /// <returns>An invalid result.</returns>
    public static Result ToInvalidResult(
        this IEnumerable<ValidationResult> validationResults,
        ResultMessageSeverity severity = ResultMessageSeverity.Warning) =>
        Results.Invalid(validationResults.ToMessages(severity));

    /// <summary>
    /// Converts validation results to a typed invalid result.
    /// </summary>
    /// <typeparam name="T">The result value type.</typeparam>
    /// <param name="validationResults">The validation results to convert.</param>
    /// <param name="severity">The message severity.</param>
    /// <returns>A typed invalid result.</returns>
    public static Result<T> ToInvalidResult<T>(
        this IEnumerable<ValidationResult> validationResults,
        ResultMessageSeverity severity = ResultMessageSeverity.Warning) =>
        Results.Invalid<T>(validationResults.ToMessages(severity));

    private static ResultMessage CreateMessage(
        string message,
        ResultMessageSeverity severity) =>
        severity switch
        {
            ResultMessageSeverity.Information => ResultMessages.Info(message),
            ResultMessageSeverity.Warning => ResultMessages.Warning(message),
            ResultMessageSeverity.Error => ResultMessages.Error(message),
            _ => throw new ArgumentOutOfRangeException(nameof(severity), severity, "Unsupported message severity.")
        };
}
