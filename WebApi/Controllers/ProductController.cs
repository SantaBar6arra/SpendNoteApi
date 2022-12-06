using Data.Models;
using Data;
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
    public class ProductController : ControllerBase
    {
        private readonly ILogger<UsersController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        private int _userId;

        public ProductController(ILogger<UsersController> logger, IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _userId = Convert.ToInt32(httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
        }

        [HttpGet("{listId}")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetAll(int listId)
        {
            var list = await _unitOfWork.ListRepository.GetById(listId);
            
            if (list == null)
                return NotFound();

            if (list.User.Id != _userId)
                return Forbid();

            var products = await _unitOfWork.ProductRepository.GetAll(listId);
            List<ProductDto> productDtos = new() { Capacity = products.Count() };

            // use mapper
            foreach (var product in products)
                productDtos.Add(new()
                {
                    Name = product.Name,
                    Id = product.Id,
                    CategoryId = product.Category.Id,
                    Price = product.Price,
                    ListId = product.List.Id,
                    AddDate = product.AddDate,
                    BuyUntilDate = product.BuyUntilDate,
                    PurchaseDate = product.PurchaseDate,
                    IsBought = product.IsBought,
                });

            return Ok(productDtos);
        }

        [HttpPut()]
        public async Task<IActionResult> Upsert([FromBody] ProductDto productDto)
        {
            var list = await _unitOfWork.ListRepository.GetById(productDto.ListId);

            if (list == null)
                return NotFound(HttpResponseReasons.UserNotFound);

            if (list.User.Id != _userId)
                return Forbid();

            var category = await _unitOfWork.ProductCategoryRepository.GetById(productDto.CategoryId);

            if (category == null)
                return Problem(HttpResponseReasons.CategoryNotFound);

            if (category.User.Id != _userId)
                return Forbid();

            Product product = new()
            {
                Id = productDto.Id,
                List = list,
                Name = productDto.Name,
                Category = category,
                Price = productDto.Price,
                AddDate = productDto.AddDate,
                BuyUntilDate = productDto.BuyUntilDate,
                PurchaseDate = productDto.PurchaseDate,
                IsBought = productDto.IsBought,
            };

            if (_unitOfWork.ProductRepository.Upsert(product))
                if (await _unitOfWork.Complete() > 0)
                    return Ok();

            return Problem();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _unitOfWork.ProductRepository.GetById(id);

            if (product == null)
                return NotFound(HttpResponseReasons.ObjToBeDeletedNotFound);

            if (product.List.User.Id != _userId)
                return Forbid(HttpResponseReasons.AccessForbidden);

            if (_unitOfWork.ProductRepository.Remove(product))
                if (await _unitOfWork.Complete() > 0)
                    return Ok();
            
            return Problem(HttpResponseReasons.SomethingWentWrong);
        }
    }
}
