using System.ComponentModel.DataAnnotations;

namespace Nestgrid.Response.Extensions.Validation;

/// <summary>
/// Provides extensions for converting data annotations validation results.
/// </summary>
public static class ValidationResultExtensions
{
    private const string DefaultValidationCode = "validation_failed";
    private const string DefaultValidationMessage = "The entity is invalid.";

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
    /// Converts validation results to messages with optional member properties and a shared code.
    /// </summary>
    /// <param name="validationResults">The validation results to convert.</param>
    /// <param name="code">The message code to use.</param>
    /// <param name="severity">The message severity.</param>
    /// <returns>The converted result messages.</returns>
    public static ResultMessage[] ToMessagesWithProperties(
        this IEnumerable<ValidationResult> validationResults,
        string code = DefaultValidationCode,
        ResultMessageSeverity severity = ResultMessageSeverity.Warning)
    {
        if (validationResults is null)
        {
            throw new ArgumentNullException(nameof(validationResults));
        }

        if (code is null)
        {
            throw new ArgumentNullException(nameof(code));
        }

        return validationResults
            .SelectMany(validationResult => CreateDetailedMessages(validationResult, code, severity))
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

    /// <summary>
    /// Converts validation results to an invalid result with optional member properties and a shared code.
    /// </summary>
    /// <param name="validationResults">The validation results to convert.</param>
    /// <param name="code">The message code to use.</param>
    /// <param name="severity">The message severity.</param>
    /// <returns>An invalid result.</returns>
    public static Result ToInvalidResultWithProperties(
        this IEnumerable<ValidationResult> validationResults,
        string code = DefaultValidationCode,
        ResultMessageSeverity severity = ResultMessageSeverity.Warning) =>
        Results.Invalid(validationResults.ToMessagesWithProperties(code, severity));

    /// <summary>
    /// Converts validation results to a typed invalid result with optional member properties and a shared code.
    /// </summary>
    /// <typeparam name="T">The result value type.</typeparam>
    /// <param name="validationResults">The validation results to convert.</param>
    /// <param name="code">The message code to use.</param>
    /// <param name="severity">The message severity.</param>
    /// <returns>A typed invalid result.</returns>
    public static Result<T> ToInvalidResultWithProperties<T>(
        this IEnumerable<ValidationResult> validationResults,
        string code = DefaultValidationCode,
        ResultMessageSeverity severity = ResultMessageSeverity.Warning) =>
        Results.Invalid<T>(validationResults.ToMessagesWithProperties(code, severity));

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

    private static IEnumerable<ResultMessage> CreateDetailedMessages(
        ValidationResult validationResult,
        string code,
        ResultMessageSeverity severity)
    {
        if (validationResult is null)
        {
            throw new ArgumentNullException(nameof(validationResult));
        }

        var message = validationResult.ErrorMessage ?? DefaultValidationMessage;
        var memberNames = validationResult.MemberNames
            .Where(memberName => !string.IsNullOrWhiteSpace(memberName))
            .ToArray();

        if (memberNames.Length == 0)
        {
            return new[] { CreateMessage(message, severity, code) };
        }

        return memberNames.Select(memberName => CreateMessage(message, severity, code, memberName));
    }

    private static ResultMessage CreateMessage(
        string message,
        ResultMessageSeverity severity,
        string code,
        string? property = null) =>
        severity switch
        {
            ResultMessageSeverity.Information => ResultMessages.Info(message, code, property),
            ResultMessageSeverity.Warning => ResultMessages.Warning(message, code, property),
            ResultMessageSeverity.Error => ResultMessages.Error(message, code, property),
            _ => throw new ArgumentOutOfRangeException(nameof(severity), severity, "Unsupported message severity.")
        };
}
