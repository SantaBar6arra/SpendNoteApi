using Data;
using Data.Models;
using Microsoft.AspNetCore.Mvc;
using WebApi.Constants;
using WebApi.Dtos;

namespace WebApi.Services.ControllerServices
{
    public class IncomeCategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly int _userId;

        public IncomeCategoryService(IUnitOfWork unitOfWork, int userId)
        {
            _unitOfWork = unitOfWork;
            _userId = userId;
        }

        public async Task<IEnumerable<IncomeCategoryDto>> GetAll()
        {
            var incomeCategories = await _unitOfWork.IncomeCategoryRepository.GetAll(_userId);
            List<IncomeCategoryDto> incomeCategoryDtos = new() { Capacity = incomeCategories.Count() };

            foreach (var incomeCategory in incomeCategories)
                incomeCategoryDtos.Add(new()
                {
                    Name = incomeCategory.Name,
                    Id = incomeCategory.Id,
                    UserId = incomeCategory.User.Id
                });

            return incomeCategoryDtos;
        }

        public async Task<ServiceResponse> Upsert(IncomeCategoryDto categoryDto)
        {
            var user = await _unitOfWork.UserRepository.GetById(_userId);

            if (user == null)
                return ServiceResponseUtils.FormUserNotFoundResponse();

            if (categoryDto.UserId != 0 && categoryDto.UserId != _userId)
                return ServiceResponseUtils.FormForbiddenResponse();

            IncomeCategory category = new()
            {
                User = user,
                Name = categoryDto.Name,
                Id = categoryDto.Id
            };

            if (_unitOfWork.IncomeCategoryRepository.Upsert(category))
                if (await _unitOfWork.Complete() > 0)
                    return ServiceResponseUtils.FormOkResponse();

            return ServiceResponseUtils.FormWrongResponse();
        }

        public async Task<ServiceResponse> Delete(int id)
        {
            var category = await _unitOfWork.IncomeCategoryRepository.GetById(id);

            if (category == null)
                return ServiceResponseUtils.FormNotFoundResponse(HttpResponseReasons.CategoryNotFound);

            if (category.User.Id != _userId)
                return ServiceResponseUtils.FormForbiddenResponse();

            if (_unitOfWork.IncomeCategoryRepository.Remove(category))
                if (await _unitOfWork.Complete() > 0)
                    return ServiceResponseUtils.FormOkResponse();

            return ServiceResponseUtils.FormWrongResponse();
        }
    }
}
