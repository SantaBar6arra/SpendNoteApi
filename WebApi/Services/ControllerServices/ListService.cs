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
        public async Task<ServiceResponse> Upsert(ListDto listDto)
        {
            var user = await _unitOfWork.UserRepository.GetById(_userId);

            if (user == null)
                return ServiceResponseUtils.FormUserNotFoundResponse();

            if (listDto.UserId != 0 && listDto.UserId != _userId)
                return ServiceResponseUtils.FormForbiddenResponse();

            List list = new()
            {
                User = user,
                Name = listDto.Name,
                Id = listDto.Id
            };

            if (_unitOfWork.ListRepository.Upsert(list))
                if (await _unitOfWork.Complete() > 0)
                    return ServiceResponseUtils.FormOkResponse();

            return ServiceResponseUtils.FormWrongResponse();
        }

        public async Task<ServiceResponse> Delete(int id)
        {
            var list = await _unitOfWork.ListRepository.GetById(id);

            if (list == null)
                return ServiceResponseUtils.FormNotFoundResponse(HttpResponseReasons.ObjToBeDeletedNotFound);

            if (list.User.Id != _userId)
                return ServiceResponseUtils.FormForbiddenResponse();

            if (_unitOfWork.ListRepository.Remove(list))
                if (await _unitOfWork.Complete() > 0)
                    return ServiceResponseUtils.FormOkResponse();

            return ServiceResponseUtils.FormWrongResponse();
        }
    }
}
