using Data;
using Data.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.Entity.Migrations.Model;
using System.Security.Claims;
using WebApi.Constants;
using WebApi.Dtos;
using WebApi.Services.ControllerServices;

namespace WebApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class IncomeCategoryController : ControllerBase
    {
        private readonly ILogger<IncomeCategoryController> _logger;
        private readonly IncomeCategoryService _service;

        public IncomeCategoryController(ILogger<IncomeCategoryController> logger, 
            IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            int userId = Convert.ToInt32(httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
            _service = new(unitOfWork, userId);
        }

        [HttpGet()]
        public async Task<ActionResult<IEnumerable<IncomeCategoryDto>>> GetAll() =>
            Ok(await _service.GetAll());

        [HttpPut()]
        public async Task<IActionResult> Upsert([FromBody] IncomeCategoryDto categoryDto)
        {
            if (await _service.Upsert(categoryDto))
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
