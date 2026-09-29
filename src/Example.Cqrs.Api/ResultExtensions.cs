using System.Net;
using Example.Cqrs.Application.Common;
using Microsoft.AspNetCore.Mvc;

namespace Example.Cqrs.Api;

public static class ResultExtensions
{
    extension<T>(Result<T> result)
    {
        public IResult ToHttpResult() =>
            result.IsSuccess
                ? Results.Ok((object?)result.Value)
                : ToProblem(result.Error!);

        public IResult ToCreatedResult(Func<T, string> locationFactory)
        {
            return result.IsFailure
                ? ToProblem(result.Error!)
                : Results.Created(locationFactory(result.Value), result.Value);
        }
    }

    private static IResult ToProblem(Error error)
    {
        var statusCode = error.Kind switch
        {
            ErrorKind.Validation => (int)HttpStatusCode.BadRequest,
            ErrorKind.NotFound => (int)HttpStatusCode.NotFound,
            ErrorKind.Conflict => (int)HttpStatusCode.Conflict,
            _ => (int)HttpStatusCode.InternalServerError
        };

        return Results.Problem(
            new ProblemDetails
            {
                Title = error.Code,
                Detail = error.Message,
                Status = statusCode,
                Type = $"https://httpstatuses.com/{statusCode}",
                Extensions =
                {
                    ["code"] = error.Code
                }
            });
    }
}