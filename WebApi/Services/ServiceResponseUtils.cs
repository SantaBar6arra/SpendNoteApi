using System.Net;
using WebApi.Constants;
using WebApi.Dtos;

namespace WebApi.Services
{
    public static class ServiceResponseUtils
    {
        public static ServiceResponse FormUserIdResponse(int userId) => new()
        {
            Data = userId,
            Message = HttpResponseReasons.UserCreated,
        };

        public static ServiceResponse FormTokenResponse(string token) => new()
        {
            Data = new TokenResult { Token = token },
        };

        public static ServiceResponse FormOkResponse() => new()
        {
            Message = "success"
        };

        public static ServiceResponse FormWrongResponse() => new()
        {
            HttpStatusCode = HttpStatusCode.BadRequest,
            Message = HttpResponseReasons.SomethingWentWrong,
        };

        public static ServiceResponse FormWrongResponse(string message) => new()
        {
            HttpStatusCode = HttpStatusCode.BadRequest,
            Message = message,
        };

        public static ServiceResponse FormForbiddenResponse() => new()
        {
            HttpStatusCode = HttpStatusCode.Forbidden,
            Message = HttpResponseReasons.AccessForbidden,
        };

        public static ServiceResponse FormUserNotFoundResponse() => new()
        {
            HttpStatusCode = HttpStatusCode.NotFound,
            Message = HttpResponseReasons.UserNotFound,
        };

        public static ServiceResponse FormNotFoundResponse(string message) => new()
        {
            HttpStatusCode = HttpStatusCode.NotFound,
            Message = message,
        };

        public static ServiceResponse FormObjectResponse(object o) => new()
        {
            Data = o,
        };
    }
}
