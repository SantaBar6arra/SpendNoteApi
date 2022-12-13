using Data;
using Data.Models;
using Microsoft.AspNetCore.Mvc;
using WebApi.Constants;
using WebApi.Dtos;

namespace WebApi.Services.ControllerServices
{
    public class IncomeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly int _userId;

        public IncomeService(IUnitOfWork unitOfWork, int userId)
        {
            _unitOfWork = unitOfWork;
            _userId = userId;
        }

        public async Task<IEnumerable<IncomeDto>> GetAll()
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

            return incomeDtos;
        }

        public async Task<ServiceResponse> Upsert(IncomeDto incomeDto)
        {
            var user = await _unitOfWork.UserRepository.GetById(_userId);

            if (user == null)
                return ServiceResponseUtils.FormUserNotFoundResponse();

            var category = await _unitOfWork.IncomeCategoryRepository.GetById(incomeDto.CategoryId);

            if (category == null)
                return ServiceResponseUtils.FormNotFoundResponse(HttpResponseReasons.CategoryNotFound);

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
                    return ServiceResponseUtils.FormOkResponse();

            return ServiceResponseUtils.FormWrongResponse();
        }

        public async Task<ServiceResponse> Delete(int id)
        {
            var income = await _unitOfWork.IncomeRepository.GetById(id);

            if (income == null)
                return ServiceResponseUtils.FormNotFoundResponse(HttpResponseReasons.ObjToBeDeletedNotFound);

            if (income.User.Id != _userId)
                return ServiceResponseUtils.FormForbiddenResponse();

            if (_unitOfWork.IncomeRepository.Remove(income))
                if (await _unitOfWork.Complete() > 0)
                    return ServiceResponseUtils.FormOkResponse();

            return ServiceResponseUtils.FormWrongResponse();
        }
    }
}
