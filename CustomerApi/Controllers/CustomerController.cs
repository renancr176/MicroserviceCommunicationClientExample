using CustomerApi.DbContexts.CustomerDb.Entities;
using CustomerApi.DbContexts.CustomerDb.Interfaces.Repositories;
using CustomerApi.Models.Requests;
using CustomerApi.Models.Responses;
using DomainCore.Enums;
using DomainCore.Models;
using Identity.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Exception = System.Exception;

namespace CustomerApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize("Bearer")]
    public class CustomerController : BaseController
    {
        private readonly ICustomerRepository _customerRepository;

        public CustomerController(IUserService userService, ICustomerRepository customerRepository) 
            : base(userService)
        {
            _customerRepository = customerRepository;
        }

        [HttpGet]
        [Authorize("Bearer", Roles = $"{nameof(RoleEnum.Admin)}")]
        [SwaggerResponse(200, Type = typeof(BaseResponse<IEnumerable<CustomerModel>?>))]
        [SwaggerResponse(400, Type = typeof(BaseResponse))]
        [SwaggerResponse(401)]
        [SwaggerResponse(403)]
        public async Task<IActionResult> GetPagedAsync([FromQuery] int page = 1, [FromQuery] int size = 50)
        {
            try
            {
                var customers = await _customerRepository.GetPagedAsync(page, size);
                return Response(customers?.Select(x => new CustomerModel()
                {
                    Id = x.Id,
                    UserId = x.UserId,
                    Name = x.Name,
                    Email = x.Email,
                    BirthDate = x.BirthDate,
                    Document = x.Document
                }));
            }
            catch (Exception e)
            {
                return Response(e);
            }
        }

        [HttpGet("{id:guid}")]
        [SwaggerResponse(200, Type = typeof(BaseResponse<CustomerModel?>))]
        [SwaggerResponse(400, Type = typeof(BaseResponse))]
        [SwaggerResponse(401)]
        [SwaggerResponse(403)]
        public async Task<IActionResult> GetByIdAsync([FromRoute] Guid id)
        {
            try
            {
                if (!await _userService.CurrentUserHasRole(RoleEnum.Admin) && id != _userService.UserId)
                    return Forbid();

                var entity = await _customerRepository.GetByIdAsync(id);

                return Response(entity != null ? new CustomerModel()
                {
                    Id = entity.Id,
                    UserId = entity.UserId,
                    Name = entity.Name,
                    Email = entity.Email,
                    BirthDate = entity.BirthDate,
                    Document = entity.Document
                } : null);
            }
            catch (Exception e)
            {
                return Response(e);
            }
        }

        [HttpPost]
        [SwaggerResponse(200, Type = typeof(BaseResponse<CustomerModel?>))]
        [SwaggerResponse(400, Type = typeof(BaseResponse))]
        [SwaggerResponse(401)]
        public async Task<IActionResult> CreateAsync([FromBody] CreateCustomerRequest request)
        {
            if (!ModelState.IsValid) return InvalidModelResponse();

            try
            {
                var entity = new Customer(
                    _userService.UserId.Value, 
                    request.Name,
                    request.BirthDate,
                    request.Document,
                    request.Email);

                await _customerRepository.InsertAsync(entity);
                await _customerRepository.SaveChangesAsync();

                return Response(new CustomerModel()
                {
                    Id = entity.Id,
                    UserId = entity.UserId,
                    Name = entity.Name,
                    Email = entity.Email,
                    BirthDate = entity.BirthDate,
                    Document = entity.Document
                });
            }
            catch (Exception e)
            {
                return Response(e);
            }
        }

        [HttpPut]
        [SwaggerResponse(200, Type = typeof(BaseResponse<CustomerModel?>))]
        [SwaggerResponse(400, Type = typeof(BaseResponse))]
        [SwaggerResponse(401)]
        public async Task<IActionResult> UpdateAsync([FromBody] UpdateCustomerRequest request)
        {
            if (!ModelState.IsValid) return InvalidModelResponse();

            try
            {
                var entity = await _customerRepository.GetByIdAsync(request.Id);

                if (entity != null 
                    && !await _userService.CurrentUserHasRole(RoleEnum.Admin) 
                    && entity.UserId != _userService.UserId)
                {
                    return Forbid();
                } else if (entity == null)
                {
                    throw new Exception("Customer not found");
                }

                entity.Name = request.Name;
                entity.BirthDate = request.BirthDate;
                entity.Document = request.Document;
                entity.Email = request.Email;

                await _customerRepository.UpdateAsync(entity);
                await _customerRepository.SaveChangesAsync();

                return Response(new CustomerModel()
                {
                    Id = entity.Id,
                    UserId = entity.UserId,
                    Name = entity.Name,
                    Email = entity.Email,
                    BirthDate = entity.BirthDate,
                    Document = entity.Document
                });
            }
            catch (Exception e)
            {
                return Response(e);
            }
        }
    }
}
