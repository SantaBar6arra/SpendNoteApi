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
        public async Task<ServiceResponse> Upsert(ProductCategoryDto categoryDto)
        {
            var user = await _unitOfWork.UserRepository.GetById(_userId);

            if (user == null)
                return ServiceResponseUtils.FormUserNotFoundResponse();

            if (categoryDto.UserId != 0 && categoryDto.UserId != _userId)
                return ServiceResponseUtils.FormForbiddenResponse();

            ProductCategory category = new()
            {
                User = user,
                Name = categoryDto.Name,
                Id = categoryDto.Id
            };

            if (_unitOfWork.ProductCategoryRepository.Upsert(category))
                if (await _unitOfWork.Complete() > 0)
                    return ServiceResponseUtils.FormOkResponse();

            return ServiceResponseUtils.FormWrongResponse();
        }

        public async Task<ServiceResponse> Delete(int id)
        {
            var category = await _unitOfWork.ProductCategoryRepository.GetById(id);

            if (category == null)
                return ServiceResponseUtils.FormNotFoundResponse(HttpResponseReasons.CategoryNotFound);

            if (category.User.Id != _userId)
                return ServiceResponseUtils.FormForbiddenResponse();

            if (_unitOfWork.ProductCategoryRepository.Remove(category))
                if (await _unitOfWork.Complete() > 0)
                    return ServiceResponseUtils.FormOkResponse();

            return ServiceResponseUtils.FormWrongResponse();
        }
    }
}
