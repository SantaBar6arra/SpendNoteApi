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
    public class ListController : ControllerBase
    {
        private readonly ILogger<UsersController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        private int _userId;

        public ListController(ILogger<UsersController> logger, IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _userId = Convert.ToInt32(httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
        }

        [HttpGet()]
        public async Task<ActionResult<IEnumerable<ListDto>>> GetAll()
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

            return Ok(listDtos);
        }

        [HttpPut()]
        public async Task<IActionResult> Upsert([FromBody] ListDto listDto)
        {
            var user = await _unitOfWork.UserRepository.GetById(_userId);

            if (user == null)
                return BadRequest(HttpResponseReasons.UserNotFound);

            List list = new()
            {
                Id = listDto.Id,
                User = user,
                Name = listDto.Name,
            };

            if (_unitOfWork.ListRepository.Upsert(list))
                if (await _unitOfWork.Complete() > 0)
                    return Ok();

            return Problem(HttpResponseReasons.SomethingWentWrong);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var list = await _unitOfWork.ListRepository.GetById(id);

            if (list == null)
                return NotFound(HttpResponseReasons.ObjToBeDeletedNotFound);

            if (list.User.Id != _userId)
                return Forbid(HttpResponseReasons.AccessForbidden);

            if (_unitOfWork.ListRepository.Remove(list))
            {
                await _unitOfWork.Complete();
                return Ok();
            }

            return Problem(HttpResponseReasons.SomethingWentWrong);
        }
    }
}
