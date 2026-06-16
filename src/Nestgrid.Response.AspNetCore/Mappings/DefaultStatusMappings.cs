using Microsoft.AspNetCore.Http;

namespace Nestgrid.Response.AspNetCore.Mappings;

internal static class DefaultStatusMappings
{
    internal static IDictionary<ResultStatus, int> Create()
    {
        return new Dictionary<ResultStatus, int>
        {
            [ResultStatus.Ok] = StatusCodes.Status200OK,
            [ResultStatus.Created] = StatusCodes.Status201Created,
            [ResultStatus.Accepted] = StatusCodes.Status202Accepted,
            [ResultStatus.NoContent] = StatusCodes.Status204NoContent,
            [ResultStatus.Invalid] = StatusCodes.Status400BadRequest,
            [ResultStatus.Unauthorized] = StatusCodes.Status401Unauthorized,
            [ResultStatus.Forbidden] = StatusCodes.Status403Forbidden,
            [ResultStatus.NotFound] = StatusCodes.Status404NotFound,
            [ResultStatus.Conflict] = StatusCodes.Status409Conflict,
            [ResultStatus.Cancelled] = StatusCodes.Status409Conflict,
            [ResultStatus.Failed] = StatusCodes.Status422UnprocessableEntity,
            [ResultStatus.Error] = StatusCodes.Status500InternalServerError
        };
    }
}
