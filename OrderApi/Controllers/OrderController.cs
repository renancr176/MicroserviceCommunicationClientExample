using System.Linq.Expressions;
using CustomerApiClient.Interfaces.Services;
using CustomerApiClient.Models.Requests;
using DomainCore.Enums;
using DomainCore.Models;
using Identity.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderApi.DbContexts.OrderDb.Entities;
using OrderApi.DbContexts.OrderDb.Interfaces.Repositories;
using OrderApi.Models;
using OrderApi.Models.Requests;
using ProductApiClient.Interfaces.Services;
using ProductApiClient.Models.Requests;
using Swashbuckle.AspNetCore.Annotations;

namespace OrderApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize("Bearer")]
    public class OrderController : BaseController
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ICustomerService _customerService;
        private readonly IProductService _productService;

        public OrderController(IUserService userService, IOrderRepository orderRepository,
            ICustomerService customerService, IProductService productService) 
            : base(userService)
        {
            _orderRepository = orderRepository;
            _customerService = customerService;
            _productService = productService;
        }

        private static IEnumerable<string> Includes = new[] { "Products" };

        [HttpGet]
        [SwaggerResponse(200, Type = typeof(BaseResponse<IEnumerable<OrderModel>?>))]
        [SwaggerResponse(400, Type = typeof(BaseResponse))]
        [SwaggerResponse(401)]
        [SwaggerResponse(403)]
        public async Task<IActionResult> GetPagedAsync([FromQuery] int page = 1, [FromQuery] int size = 50)
        {
            try
            {
                var admin = await _userService.CurrentUserHasRole(RoleEnum.Admin);
                var customer =
                    (await _customerService.SearchAsync(new CustomerSearchRequest(userId: _userService.UserId)))
                    .Data
                    ?.FirstOrDefault();

                var entities = await _orderRepository.GetPagedAsync(
                    page, 
                    size, 
                    includes: Includes,
                    o => 
                    (admin || o.CustomerId == customer.Id),
                    ordenations: new Dictionary<Expression<Func<Order, object>>, OrderByEnum>()
                    {
                        {o => o.Id, OrderByEnum.Descending}
                    });
                return Response(entities?.Select(o => new OrderModel()
                {
                    Id = o.Id,
                    CustomerId = o.CustomerId,
                    Total = o.Total,
                    Products = o.Products.Select(p => new ProductModel()
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Price = p.Price,
                        Quantity = p.Quantity,
                        Total = p.Total
                    })
                }));
            }
            catch (Exception e)
            {
                return Response(e);
            }
        }

        [HttpGet("{id:int}")]
        [SwaggerResponse(200, Type = typeof(BaseResponse<OrderModel?>))]
        [SwaggerResponse(400, Type = typeof(BaseResponse))]
        [SwaggerResponse(401)]
        [SwaggerResponse(403)]
        public async Task<IActionResult> GetByIdAsync([FromRoute] int id)
        {
            try
            {
                var entity = await _orderRepository.GetByIdAsync(id, Includes);

                if (entity != null)
                {
                    var customer = await _customerService.GetByIdAsync(entity.CustomerId);
                    if (!await _userService.CurrentUserHasRole(RoleEnum.Admin) 
                        && customer.Data?.UserId != _userService.UserId)
                        return Forbid();
                }

                return Response(entity != null ? new OrderModel()
                {
                    Id = entity.Id,
                    CustomerId = entity.CustomerId,
                    Total = entity.Total,
                    Products = entity.Products.Select(p => new ProductModel()
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Price = p.Price,
                        Quantity = p.Quantity,
                        Total = p.Total
                    })
                } : null);
            }
            catch (Exception e)
            {
                return Response(e);
            }
        }

        [HttpPost]
        [SwaggerResponse(200, Type = typeof(BaseResponse<OrderModel?>))]
        [SwaggerResponse(400, Type = typeof(BaseResponse))]
        [SwaggerResponse(401)]
        public async Task<IActionResult> CreateAsync([FromBody] CreateOrderRequest request)
        {
            if (!ModelState.IsValid) return InvalidModelResponse();

            try
            {
                //TODO: Check and get customer by logued user _userService.UserId
                var customer =
                    (await _customerService
                        .SearchAsync(new CustomerSearchRequest(userId: _userService.UserId)))
                    .Data.FirstOrDefault();

                if (customer == null)
                    return BadRequest(new BaseResponse()
                    {
                        Errors = new List<BaseResponseError>()
                        {
                            new BaseResponseError()
                            {
                                ErrorCode = "CustomerNotFound",
                                Message = "Customer not found."
                            }
                        }
                    });

                //TODO: Check and get produts by request Product.Id
                var  products = await _productService.SearchAsync(
                    new ProductSearchRequest(
                        size: request.Products.Count(),
                        ids: request.Products.Select(x => x.Id),
                        active: true));

                if (products.Data.Count() != request.Products.Count())
                    return BadRequest(new BaseResponse()
                    {
                        Errors = request.Products
                            .Where(x => !products.Data.Any(p => p.Id == x.Id))
                            .Select(x => new BaseResponseError()
                            {
                                ErrorCode = "ProductNotFound",
                                Message = $"The product of ID {x.Id} not exists or it is not active."
                            }).ToList()
                    });

                var entity = new Order(
                    customer.Id, 
                    products.Data.Select(p => 
                        new Product(
                            p.Id,
                            p.Name, 
                            p.Price,
                            request.Products.FirstOrDefault(x => x.Id == p.Id).Quantity
                        )
                    ).ToList());

                await _orderRepository.InsertAsync(entity);
                await _orderRepository.SaveChangesAsync();

                return Response(entity != null ? new OrderModel()
                {
                    Id = entity.Id,
                    CustomerId = entity.CustomerId,
                    Total = entity.Total,
                    Products = entity.Products.Select(p => new ProductModel()
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Price = p.Price,
                        Quantity = p.Quantity,
                        Total = p.Total
                    })
                } : null);
            }
            catch (Exception e)
            {
                return Response(e);
            }
        }

        [HttpDelete("{id:int}")]
        [SwaggerResponse(200, Type = typeof(BaseResponse<OrderModel?>))]
        [SwaggerResponse(400, Type = typeof(BaseResponse))]
        [SwaggerResponse(401)]
        public async Task<IActionResult> DeleteAsync([FromRoute] int id)
        {
            if (!ModelState.IsValid) return InvalidModelResponse();

            try
            {
                var entity = await _orderRepository.GetByIdAsync(id);

                if (entity != null)
                {
                    //TODO: Check and get customer by logued user _userService.UserId
                    var customer =
                        await _customerService.SearchAsync(new CustomerSearchRequest(userId: _userService.UserId));
                    if (!await _userService.CurrentUserHasRole(RoleEnum.Admin) 
                        && entity.CustomerId != customer.Data?.FirstOrDefault()?.Id)
                        return Forbid();
                } else
                {
                    throw new Exception("Order not found.");
                }

                await _orderRepository.DeleteAsync(id);
                await _orderRepository.SaveChangesAsync();

                return Response();
            }
            catch (Exception e)
            {
                return Response(e);
            }
        }
    }
}
