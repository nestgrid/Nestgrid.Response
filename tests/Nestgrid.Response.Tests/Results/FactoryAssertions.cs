using System.Reflection;
using Shouldly;

namespace Nestgrid.Response.Tests.Results;

internal static class FactoryAssertions
{
    public static void ShouldHaveStatus(Result result, ResultStatus status)
    {
        result.Status.ShouldBe(status);
        result.Messages.ShouldBeEmpty();
    }

    public static void ShouldHaveMessage(Result result, ResultStatus status, ResultMessage message)
    {
        result.Status.ShouldBe(status);
        result.Messages.ShouldBe([message]);
    }

    public static void ShouldHaveErrorMessage(Result result, ResultStatus status, string message)
    {
        result.Status.ShouldBe(status);
        result.Messages.Count.ShouldBe(1);
        result.Messages[0].Message.ShouldBe(message);
        result.Messages[0].Severity.ShouldBe(ResultMessageSeverity.Error);
    }

    public static void ShouldHaveValue<T>(Result<T> result, ResultStatus status, T value)
    {
        result.Status.ShouldBe(status);
        result.Value.ShouldBe(value);
        result.Messages.ShouldBeEmpty();
    }

    public static void ShouldHaveValueAndMessage<T>(Result<T> result, ResultStatus status, T value, ResultMessage message)
    {
        result.Status.ShouldBe(status);
        result.Value.ShouldBe(value);
        result.Messages.ShouldBe([message]);
    }

    public static void ShouldHaveDefaultValue<T>(Result<T> result, ResultStatus status)
    {
        result.Status.ShouldBe(status);
        result.Value.ShouldBe(default(T));
        result.Messages.ShouldBeEmpty();
    }

    public static void ShouldNotHaveStringOverload(string methodName)
    {
        var overload = typeof(ResultsFactory)
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
            .SingleOrDefault(method => method.Name == methodName && HasSingleStringParameter(method));

        overload.ShouldBeNull();
    }

    private static bool HasSingleStringParameter(MethodInfo method)
    {
        var parameters = method.GetParameters();

        return parameters.Length == 1 && parameters[0].ParameterType == typeof(string);
    }
}
