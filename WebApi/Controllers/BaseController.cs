using Microsoft.AspNetCore.Mvc;
using System.Net;
using WebApi.Dtos;

namespace WebApi.Controllers
{ 
    [ApiController]
    public class BaseController : ControllerBase
    {
        [NonAction]
        protected IActionResult HandleServiceResponse(ServiceResponse serviceResponse)
        {
            return serviceResponse.HttpStatusCode switch
            {
                HttpStatusCode.OK => Ok(serviceResponse.Data),
                HttpStatusCode.BadRequest => BadRequest(serviceResponse.Message),
                HttpStatusCode.Forbidden => Forbid(serviceResponse.Message),
                HttpStatusCode.NotFound => NotFound(serviceResponse.Message),
                _ => Problem(serviceResponse.Message),
            };
        }
    }
}
