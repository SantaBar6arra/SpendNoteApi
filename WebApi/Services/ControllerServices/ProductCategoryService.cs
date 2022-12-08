using Data;
using Data.Models;
using Microsoft.AspNetCore.Mvc;
using WebApi.Constants;
using WebApi.Dtos;

namespace WebApi.Services.ControllerServices
{
    public class ProductCategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly int _userId;

        public ProductCategoryService(IUnitOfWork unitOfWork, int userId)
        {
            _unitOfWork = unitOfWork;
            _userId = userId;
        }

        [HttpGet()]
        public async Task<IEnumerable<ProductCategoryDto>> GetAll()
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

            return productCategoryDtos;
        }

        [HttpPut()]
        public async Task<bool> Upsert(ProductCategoryDto productCategoryDto)
        {
            var user = await _unitOfWork.UserRepository.GetById(_userId);

            if (user == null)
                return false;

            ProductCategory productCategory = new()
            {
                Id = productCategoryDto.Id,
                User = user,
                Name = productCategoryDto.Name,
            };

            if (_unitOfWork.ProductCategoryRepository.Upsert(productCategory))
                if (await _unitOfWork.Complete() > 0)
                    return true;

            return false;
        }

        public async Task<bool> Delete(int id)
        {
            var category = await _unitOfWork.ProductCategoryRepository.GetById(id);

            if (category == null)
                return false;

            if (category.User.Id != _userId)
                return false;

            if (_unitOfWork.ProductCategoryRepository.Remove(category))
                if (await _unitOfWork.Complete() > 0)
                    return true;

            return false;
        }
    }
}
