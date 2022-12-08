using Data;
using Data.Models;
using Microsoft.AspNetCore.Mvc;
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

        public async Task<bool> Upsert(IncomeDto incomeDto)
        {
            var user = await _unitOfWork.UserRepository.GetById(_userId);

            if (user == null)
                return false;

            var category = await _unitOfWork.IncomeCategoryRepository.GetById(incomeDto.CategoryId);

            if (category == null)
                return false;

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
                    return true;

            return false;
        }

        public async Task<bool> Delete(int id)
        {
            var income = await _unitOfWork.IncomeRepository.GetById(id);

            if (income == null)
                return false;

            if (income.User.Id != _userId)
                return false;

            if (_unitOfWork.IncomeRepository.Remove(income))
                if (await _unitOfWork.Complete() > 0)
                    return true;

            return false;
        }
    }
}
