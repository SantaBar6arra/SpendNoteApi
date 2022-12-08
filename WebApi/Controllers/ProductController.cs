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
    public class ProductController : ControllerBase
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
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetAll(int listId)
        {
            var productCategories = await _service.GetAll(listId);

            if (productCategories == null)
                return BadRequest(HttpResponseReasons.SomethingWentWrong);
            return Ok(productCategories);
        }

        [HttpPut()]
        public async Task<IActionResult> Upsert([FromBody] ProductDto productDto)
        {
            if (await _service.Upsert(productDto))
                return Ok();
            return BadRequest(HttpResponseReasons.SomethingWentWrong);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (await _service.Delete(id))
                return Ok();
            return BadRequest(HttpResponseReasons.SomethingWentWrong);
        }
    }
}
