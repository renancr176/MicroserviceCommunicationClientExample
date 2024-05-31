using DomainCore.Enums;
using DomainCore.Models;
using Identity.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderApi.DbContexts.OrderDb.Entities;
using OrderApi.DbContexts.OrderDb.Interfaces.Repositories;
using OrderApi.Models;
using OrderApi.Models.Requests;
using Swashbuckle.AspNetCore.Annotations;

namespace OrderApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize("Bearer")]
    public class OrderController : BaseController
    {
        private readonly IOrderRepository _orderRepository;

        public OrderController(IUserService userService, IOrderRepository orderRepository) 
            : base(userService)
        {
            _orderRepository = orderRepository;
        }

        private static IEnumerable<string> Includes = new[] { "Products" };

        [HttpGet]
        [Authorize("Bearer", Roles = $"{nameof(RoleEnum.Admin)}")]
        [SwaggerResponse(200, Type = typeof(BaseResponse<IEnumerable<OrderModel>?>))]
        [SwaggerResponse(400, Type = typeof(BaseResponse))]
        [SwaggerResponse(401)]
        [SwaggerResponse(403)]
        public async Task<IActionResult> GetPagedAsync([FromQuery] int page = 1, [FromQuery] int size = 50)
        {
            try
            {
                var entities = await _orderRepository.GetPagedAsync(page, size, includes: Includes);
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

        [HttpGet("{id:guid}")]
        [SwaggerResponse(200, Type = typeof(BaseResponse<OrderModel?>))]
        [SwaggerResponse(400, Type = typeof(BaseResponse))]
        [SwaggerResponse(401)]
        [SwaggerResponse(403)]
        public async Task<IActionResult> GetByIdAsync([FromRoute] Guid id)
        {
            try
            {
                var entity = await _orderRepository.GetByIdAsync(id, Includes);

                //if (!await _userService.CurrentUserHasRole(RoleEnum.Admin) && id != _userService.UserId)
                //    return Forbid();

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
                //TODO: Check and get customer by loged user _userService.UserId
                var customerId = Guid.Empty;
                //TODO: Check and get produts by request Product.Id
                var  products = new List<Product>();

                var entity = new Order(customerId, products);

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

        [HttpDelete("{id:guid}")]
        [SwaggerResponse(200, Type = typeof(BaseResponse<OrderModel?>))]
        [SwaggerResponse(400, Type = typeof(BaseResponse))]
        [SwaggerResponse(401)]
        public async Task<IActionResult> DeleteAsync([FromRoute] Guid id)
        {
            if (!ModelState.IsValid) return InvalidModelResponse();

            try
            {
                var entity = await _orderRepository.GetByIdAsync(id);

                if (entity == null)
                {
                    throw new Exception("Order not found.");
                }

                //TODO: Check and get customer by loged user _userService.UserId
                var customerId = Guid.Empty;

                if (!await _userService.CurrentUserHasRole(RoleEnum.Admin) && entity.CustomerId != customerId)
                    return Forbid();

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
