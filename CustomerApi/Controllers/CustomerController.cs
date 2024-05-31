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

                var customer = await _customerRepository.GetByIdAsync(id);

                return Response(customer != null ? new CustomerModel()
                {
                    Id = customer.Id,
                    Name = customer.Name,
                    Email = customer.Email,
                    BirthDate = customer.BirthDate,
                    Document = customer.Document
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
                var customer = new Customer(
                    _userService.UserId.Value, 
                    request.Name,
                    request.BirthDate,
                    request.Document,
                    request.Email);

                await _customerRepository.InsertAsync(customer);
                await _customerRepository.SaveChangesAsync();

                return Response(new CustomerModel()
                {
                    Id = customer.Id,
                    Name = customer.Name,
                    Email = customer.Email,
                    BirthDate = customer.BirthDate,
                    Document = customer.Document
                });
            }
            catch (Exception e)
            {
                return Response(e);
            }
        }
    }
}
