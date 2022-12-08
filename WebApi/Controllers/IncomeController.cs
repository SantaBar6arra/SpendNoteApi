using Data;
using Data.Models;
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
    public class IncomeController : ControllerBase
    {
        private readonly ILogger<UsersController> _logger;
        private readonly IncomeService _service;

        public IncomeController(ILogger<UsersController> logger, IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            int userId = Convert.ToInt32(httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
            _service = new(unitOfWork, userId);
        }

        [HttpGet()]
        public async Task<ActionResult<IEnumerable<IncomeDto>>> GetAll() =>
            Ok(await _service.GetAll());

        [HttpPut()]
        public async Task<IActionResult> Upsert([FromBody] IncomeDto incomeDto)
        {
            if (await _service.Upsert(incomeDto))
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
