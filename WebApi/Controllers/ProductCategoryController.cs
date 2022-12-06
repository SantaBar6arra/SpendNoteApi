using Data;
using Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebApi.Constants;
using WebApi.Dtos;

namespace WebApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class ProductCategoryController : ControllerBase
    {
        private readonly ILogger<UsersController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        private int _userId;

        public ProductCategoryController(ILogger<UsersController> logger, IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _userId = Convert.ToInt32(httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
        }

        [HttpGet()]
        public async Task<ActionResult<IEnumerable<ProductCategoryDto>>> GetAll()
        {
            var productCategories = await _unitOfWork.ProductCategoryRepository.GetAll(_userId);
            List<ProductCategoryDto> productCategoryDtos = new() { Capacity = productCategories.Count() };

            // use mapper
            foreach (var productCategory in productCategories)
                productCategoryDtos.Add(new()
                {
                    Name = productCategory.Name,
                    Id = productCategory.Id,
                    UserId = productCategory.User.Id
                });

            return Ok(productCategoryDtos);
        }

        [HttpPut()]
        public async Task<IActionResult> Upsert([FromBody] ProductCategoryDto productCategoryDto)
        {
            var user = await _unitOfWork.UserRepository.GetById(_userId);

            if (user == null)
                return BadRequest(HttpResponseReasons.UserNotFound); 

            ProductCategory productCategory = new()
            {
                Id = productCategoryDto.Id,
                User = user,
                Name = productCategoryDto.Name,
            };

            if (_unitOfWork.ProductCategoryRepository.Upsert(productCategory))
                if (await _unitOfWork.Complete() > 0)
                    return Ok();

            return Problem(HttpResponseReasons.SomethingWentWrong);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _unitOfWork.ProductCategoryRepository.GetById(id);

            if (category == null)
                return NotFound(HttpResponseReasons.ObjToBeDeletedNotFound);

            if (category.User.Id != _userId)
                return Forbid(HttpResponseReasons.AccessForbidden);

            if (_unitOfWork.ProductCategoryRepository.Remove(category))
            {
                await _unitOfWork.Complete();
                return Ok();
            }

            return Problem(HttpResponseReasons.SomethingWentWrong);
        }
    }
}
