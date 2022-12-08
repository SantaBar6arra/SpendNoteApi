using Data;
using Data.Models;
using Microsoft.AspNetCore.Mvc;
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
        public async Task<IEnumerable<ProductDto>?> GetAll(int listId)
        {
            var list = await _unitOfWork.ListRepository.GetById(listId);

            if (list == null)
                return null;

            if (list.User.Id != _userId)
                return null;

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

            return productDtos;
        }

        public async Task<bool> Upsert(ProductDto productDto)
        {
            var list = await _unitOfWork.ListRepository.GetById(productDto.ListId);

            if (list == null)
                return false;

            if (list.User.Id != _userId)
                return false;

            var category = await _unitOfWork.ProductCategoryRepository.GetById(productDto.CategoryId);

            if (category == null)
                return false;

            if (category.User.Id != _userId)
                return false;

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
                    return true;

            return false;
        }

        public async Task<bool> Delete(int id)
        {
            var product = await _unitOfWork.ProductRepository.GetById(id);

            if (product == null)
                return false;

            if (product.List.User.Id != _userId)
                return false;

            if (_unitOfWork.ProductRepository.Remove(product))
                if (await _unitOfWork.Complete() > 0)
                    return true;

            return false;
        }
    }
}
