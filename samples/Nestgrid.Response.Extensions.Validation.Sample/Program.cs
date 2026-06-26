using System.ComponentModel.DataAnnotations;
using Nestgrid.Response;
using Nestgrid.Response.Extensions.Validation;

var validUser = new CreateUserRequest("Ada Lovelace", "ada@example.test");
Validator.ValidateObject(
    validUser,
    new ValidationContext(validUser),
    validateAllProperties: true);

Console.WriteLine("Validator.ValidateObject completed for a valid model.");

var invalidUser = new CreateUserRequest("", "not-an-email");
var validationResults = new List<ValidationResult>();

Validator.TryValidateObject(
    invalidUser,
    new ValidationContext(invalidUser),
    validationResults,
    validateAllProperties: true);

var firstValidationResult = validationResults[0];
var message = firstValidationResult.ToMessage();
var messages = validationResults.ToMessages();
var invalidResult = firstValidationResult.ToInvalidResult();
var typedInvalidResult = validationResults.ToInvalidResult<UserDto>();

PrintMessage("ValidationResult.ToMessage", message);
PrintMessages("ValidationResult.ToMessages", messages);
PrintResult("ValidationResult.ToInvalidResult", invalidResult);
PrintResult("ValidationResult.ToInvalidResult<T>", typedInvalidResult);

static void PrintMessage(string label, ResultMessage message)
{
    Console.WriteLine();
    Console.WriteLine(label);
    Console.WriteLine($"  {message.Severity}: {message.Message}");
}

static void PrintMessages(string label, IEnumerable<ResultMessage> messages)
{
    Console.WriteLine();
    Console.WriteLine(label);

    foreach (var message in messages)
    {
        Console.WriteLine($"  {message.Severity}: {message.Message}");
    }
}

static void PrintResult(string label, Result result)
{
    Console.WriteLine();
    Console.WriteLine($"{label}: {result.Status}");

    foreach (var message in result.Messages)
    {
        Console.WriteLine($"  {message.Severity}: {message.Message}");
    }
}

record CreateUserRequest(
    [property: Required] string Name,
    [property: EmailAddress] string Email);

record UserDto(int Id, string Name, string Email);
