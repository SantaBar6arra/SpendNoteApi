using Data;
using Data.Models;
using Microsoft.AspNetCore.Mvc;
using WebApi.Constants;
using WebApi.Dtos;

namespace WebApi.Services.ControllerServices
{
    public class ListService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly int _userId;

        public ListService(IUnitOfWork unitOfWork, int userId)
        {
            _unitOfWork = unitOfWork;
            _userId = userId;
        }

        public async Task<IEnumerable<ListDto>> GetAll()
        {
            var lists = await _unitOfWork.ListRepository.GetAll(_userId);
            List<ListDto> listDtos = new() { Capacity = lists.Count() };

            // use mapper
            foreach (var list in lists)
                listDtos.Add(new()
                {
                    Name = list.Name,
                    Id = list.Id,
                    UserId = list.User.Id
                });

            return listDtos;
        }

        [HttpPut()]
        public async Task<bool> Upsert(ListDto listDto)
        {
            var user = await _unitOfWork.UserRepository.GetById(_userId);

            if (user == null)
                return false;

            List list = new()
            {
                Id = listDto.Id,
                User = user,
                Name = listDto.Name,
            };

            if (_unitOfWork.ListRepository.Upsert(list))
                if (await _unitOfWork.Complete() > 0)
                    return true;

            return false;
        }

        public async Task<bool> Delete(int id)
        {
            var list = await _unitOfWork.ListRepository.GetById(id);

            if (list == null)
                return false;

            if (list.User.Id != _userId)
                return false;

            if (_unitOfWork.ListRepository.Remove(list))
                if (await _unitOfWork.Complete() > 0)
                    return true;

            return false;
        }
    }
}
