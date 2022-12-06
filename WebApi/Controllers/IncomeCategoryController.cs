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

namespace WebApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class IncomeCategoryController : ControllerBase
    {
        private readonly ILogger<UsersController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        private int _userId;

        public IncomeCategoryController(ILogger<UsersController> logger, IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _userId = Convert.ToInt32(httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
        }

        [HttpGet()]
        public async Task<ActionResult<IEnumerable<IncomeCategoryDto>>> GetAll()
        {
            var incomeCategories = await _unitOfWork.IncomeCategoryRepository.GetAll(_userId);
            List<IncomeCategoryDto> incomeCategoryDtos = new() { Capacity = incomeCategories.Count() };
            // use mapper
            foreach (var incomeCategory in incomeCategories)
                incomeCategoryDtos.Add(new () 
                {
                    Name = incomeCategory.Name,       
                    Id = incomeCategory.Id,
                    UserId = incomeCategory.User.Id    
                });

            return Ok(incomeCategoryDtos);
        }

        [HttpPut()]
        public async Task<IActionResult> Upsert([FromBody] IncomeCategoryDto categoryDto)
        {
            var user = await _unitOfWork.UserRepository.GetById(_userId);

            if (user == null)
                return NotFound(HttpResponseReasons.UserNotFound);
               
            if (categoryDto.UserId != 0 && categoryDto.UserId != _userId)
                return Forbid();

            IncomeCategory category = new()
            {
                User = user,
                Name = categoryDto.Name,
                Id = categoryDto.Id
            };

            if (_unitOfWork.IncomeCategoryRepository.Upsert(category))
                if (await _unitOfWork.Complete() > 0)
                    return Ok();

            return Problem();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _unitOfWork.IncomeCategoryRepository.GetById(id);

            if (category == null)
                return NotFound(HttpResponseReasons.ObjToBeDeletedNotFound);
                
            if (category.User.Id != _userId)
                return Forbid(HttpResponseReasons.AccessForbidden);

            if (_unitOfWork.IncomeCategoryRepository.Remove(category))
                if (await _unitOfWork.Complete() > 0)
                    return Ok();
            
            return BadRequest();
        }
    }
}
