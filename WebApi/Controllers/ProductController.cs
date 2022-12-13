using Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebApi.Constants;
using WebApi.Dtos;
using WebApi.Services.ControllerServices;

namespace WebApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class ProductController : BaseController
    {
        private readonly ILogger<UsersController> _logger;
        private readonly ProductService _service;

        public ProductController(ILogger<UsersController> logger, IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            int userId = Convert.ToInt32(httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
            _service = new(unitOfWork, userId);
        }

        [HttpGet("{listId}")]
        public async Task<IActionResult> GetAll(int listId)
        {
            var serviceResponse = await _service.GetAll(listId);
            return HandleServiceResponse(serviceResponse);
        }

        [HttpPut()]
        public async Task<IActionResult> Upsert([FromBody] ProductDto productDto)
        {
            var serviceResponse = await _service.Upsert(productDto);
            return HandleServiceResponse(serviceResponse);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var serviceResponse = await _service.Delete(id);
            return HandleServiceResponse(serviceResponse);
        }
    }
}
