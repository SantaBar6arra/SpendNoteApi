using System.Net;

namespace WebApi.Dtos
{
    public class ServiceResponse
    {
        public object? Data { get; set; } 
        public HttpStatusCode HttpStatusCode { get; set; } = HttpStatusCode.OK;
        public string Message { get; set; } = string.Empty;
    }
}
