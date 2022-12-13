using Data;
using Data.Models;
using Microsoft.AspNetCore.Mvc;
using WebApi.Constants;
using WebApi.Dtos;

namespace WebApi.Services.ControllerServices
{
    public class ProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly int _userId;

        public ProductService(IUnitOfWork unitOfWork, int userId)
        {
            _unitOfWork = unitOfWork;
            _userId = userId;
        }

        [HttpGet("{listId}")]
        public async Task<ServiceResponse> GetAll(int listId)
        {
            var list = await _unitOfWork.ListRepository.GetById(listId);

            if (list == null)
                return ServiceResponseUtils.FormNotFoundResponse("list not found");

            if (list.User.Id != _userId)
                return ServiceResponseUtils.FormForbiddenResponse();

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

            return ServiceResponseUtils.FormObjectResponse(productDtos);
        }

        public async Task<ServiceResponse> Upsert(ProductDto productDto)
        {
            var list = await _unitOfWork.ListRepository.GetById(productDto.ListId);

            if (list == null)
                return ServiceResponseUtils.FormNotFoundResponse("list not found");

            if (list.User.Id != _userId)
                return ServiceResponseUtils.FormForbiddenResponse();

            var category = await _unitOfWork.ProductCategoryRepository.GetById(productDto.CategoryId);

            if (category == null)
                return ServiceResponseUtils.FormNotFoundResponse(HttpResponseReasons.CategoryNotFound);

            if (category.User.Id != _userId)
                return ServiceResponseUtils.FormForbiddenResponse();

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
                    return ServiceResponseUtils.FormOkResponse();

            return ServiceResponseUtils.FormWrongResponse();
        }

        public async Task<ServiceResponse> Delete(int id)
        {
            var product = await _unitOfWork.ProductRepository.GetById(id);

            if (product == null)
                return ServiceResponseUtils.FormNotFoundResponse(HttpResponseReasons.ObjToBeDeletedNotFound);

            if (product.List.User.Id != _userId)
                return ServiceResponseUtils.FormForbiddenResponse();

            if (_unitOfWork.ProductRepository.Remove(product))
                if (await _unitOfWork.Complete() > 0)
                    return ServiceResponseUtils.FormOkResponse();

            return ServiceResponseUtils.FormWrongResponse();
        }
    }
}
