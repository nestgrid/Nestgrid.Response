namespace Nestgrid.Response.Http.Mappings;

/// <summary>
/// Provides default HTTP status mappings for result statuses.
/// </summary>
public static class DefaultStatusMappings
{
    /// <summary>
    /// Creates the default HTTP status mappings.
    /// </summary>
    /// <returns>The default status mappings.</returns>
    public static IDictionary<ResultStatus, int> Create()
    {
        return new Dictionary<ResultStatus, int>
        {
            [ResultStatus.Ok] = 200,
            [ResultStatus.Created] = 201,
            [ResultStatus.Accepted] = 202,
            [ResultStatus.NoContent] = 204,
            [ResultStatus.Invalid] = 400,
            [ResultStatus.Unauthorized] = 401,
            [ResultStatus.Forbidden] = 403,
            [ResultStatus.NotFound] = 404,
            [ResultStatus.Conflict] = 409,
            [ResultStatus.Cancelled] = 409,
            [ResultStatus.Failed] = 422,
            [ResultStatus.Error] = 500
        };
    }
}
