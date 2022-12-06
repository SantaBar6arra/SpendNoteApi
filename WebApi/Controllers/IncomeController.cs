using Data;
using Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using System.Security.Claims;
using WebApi.Constants;
using WebApi.Dtos;

namespace WebApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class IncomeController : ControllerBase
    {
        private readonly ILogger<UsersController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        
        private int _userId;

        public IncomeController(ILogger<UsersController> logger, IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _userId = Convert.ToInt32(httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
        }

        [HttpGet()]
        public async Task<ActionResult<IEnumerable<IncomeDto>>> GetAll()
        {
            var incomes = await _unitOfWork.IncomeRepository.GetAll(_userId);
            List<IncomeDto> incomeDtos = new() { Capacity = incomes.Count() };

            // use mapper
            foreach (var income in incomes)
                incomeDtos.Add(new()
                {
                    Name = income.Name,
                    Id = income.Id,
                    Amount = income.Amount,
                    CategoryId = income.Category.Id,
                    UserId = income.User.Id,
                    AddTime = income.AddTime,
                    ExpirationTime = income.ExpirationTime
                });

            return Ok(incomeDtos);
        }

        [HttpPut()]
        public async Task<IActionResult> Upsert([FromBody] IncomeDto incomeDto)
        {
            var user = await _unitOfWork.UserRepository.GetById(_userId);

            if (user == null)
                return NotFound(HttpResponseReasons.UserNotFound); // no such user found;
            
            var category = await _unitOfWork.IncomeCategoryRepository.GetById(incomeDto.CategoryId);

            if (category == null)
                return NotFound(HttpResponseReasons.CategoryNotFound); //return "no incomecategory found"
            
            Income income = new()
            {
                Id = incomeDto.Id,
                User = user,
                Name = incomeDto.Name,
                Amount = incomeDto.Amount,
                Category = category,
                AddTime = incomeDto.AddTime,
                ExpirationTime = incomeDto.ExpirationTime,
            };

            if (_unitOfWork.IncomeRepository.Upsert(income))
                if (await _unitOfWork.Complete() > 0)
                    return Ok();

            return Problem();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var income = await _unitOfWork.IncomeRepository.GetById(id);

            if (income == null)
                return NotFound(HttpResponseReasons.ObjToBeDeletedNotFound);

            if (income.User.Id != _userId)
                return Forbid(HttpResponseReasons.AccessForbidden);

            if (_unitOfWork.IncomeRepository.Remove(income))
                if (await _unitOfWork.Complete() > 0)
                    return Ok();

            return Problem(HttpResponseReasons.SomethingWentWrong);
        }
    }
}
