using DomainCore.Enums;
using DomainCore.Models;
using Identity.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductApi.DbContexts.ProductDb.Entities;
using ProductApi.DbContexts.ProductDb.Interfaces.Repositories;
using ProductApi.Models;
using ProductApi.Models.Requests;
using Swashbuckle.AspNetCore.Annotations;

namespace ProductApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize("Bearer", Roles = $"{nameof(RoleEnum.Admin)}")]
    public class ProductController : BaseController
    {
        private readonly IProductRepository _productRepository;

        public ProductController(IUserService userService, IProductRepository productRepository) 
            : base(userService)
        {
            _productRepository = productRepository;
        }

        [HttpGet]
        [AllowAnonymous]
        [SwaggerResponse(200, Type = typeof(BaseResponse<IEnumerable<ProductModel>?>))]
        [SwaggerResponse(400, Type = typeof(BaseResponse))]
        [SwaggerResponse(401)]
        [SwaggerResponse(403)]
        public async Task<IActionResult> GetPagedAsync([FromQuery] int page = 1, [FromQuery] int size = 50)
        {
            try
            {
                var entities = await _productRepository.GetPagedAsync(page, size);
                return Response(entities?.Select(x => new ProductModel()
                {
                    Id = x.Id,
                    Name = x.Name,
                    Price = x.Price,
                    Active = x.Active
                }));
            }
            catch (Exception e)
            {
                return Response(e);
            }
        }

        [HttpGet("{id:guid}")]
        [AllowAnonymous]
        [SwaggerResponse(200, Type = typeof(BaseResponse<ProductModel?>))]
        [SwaggerResponse(400, Type = typeof(BaseResponse))]
        [SwaggerResponse(401)]
        [SwaggerResponse(403)]
        public async Task<IActionResult> GetByIdAsync([FromRoute] Guid id)
        {
            try
            {
                var entity = await _productRepository.GetByIdAsync(id);

                return Response(entity != null ? new ProductModel()
                {
                    Id = entity.Id,
                    Name = entity.Name,
                    Price = entity.Price,
                    Active = entity.Active
                } : null);
            }
            catch (Exception e)
            {
                return Response(e);
            }
        }

        [HttpPost]
        [SwaggerResponse(200, Type = typeof(BaseResponse<ProductModel?>))]
        [SwaggerResponse(400, Type = typeof(BaseResponse))]
        [SwaggerResponse(401)]
        public async Task<IActionResult> CreateAsync([FromBody] CreateProductRequest request)
        {
            if (!ModelState.IsValid) return InvalidModelResponse();

            try
            {
                var entity = new Product(request.Name, request.Price, request.Active);

                await _productRepository.InsertAsync(entity);
                await _productRepository.SaveChangesAsync();

                return Response(new ProductModel()
                {
                    Id = entity.Id,
                    Name = entity.Name,
                    Price = entity.Price,
                    Active = entity.Active
                });
            }
            catch (Exception e)
            {
                return Response(e);
            }
        }

        [HttpPut]
        [SwaggerResponse(200, Type = typeof(BaseResponse<ProductModel?>))]
        [SwaggerResponse(400, Type = typeof(BaseResponse))]
        [SwaggerResponse(401)]
        public async Task<IActionResult> UpdateAsync([FromBody] UpdateProductRequest request)
        {
            if (!ModelState.IsValid) return InvalidModelResponse();

            try
            {
                var entity = await _productRepository.GetByIdAsync(request.Id);

                if (entity == null)
                {
                    throw new Exception("Product not found.");
                }

                entity.Name = request.Name;
                entity.Price = request.Price;
                entity.Active = request.Active;

                await _productRepository.UpdateAsync(entity);
                await _productRepository.SaveChangesAsync();

                return Response(new ProductModel()
                {
                    Id = entity.Id,
                    Name = entity.Name,
                    Price = entity.Price,
                    Active = entity.Active
                });
            }
            catch (Exception e)
            {
                return Response(e);
            }
        }

        [HttpDelete("{id:guid}")]
        [SwaggerResponse(200, Type = typeof(BaseResponse<ProductModel?>))]
        [SwaggerResponse(400, Type = typeof(BaseResponse))]
        [SwaggerResponse(401)]
        public async Task<IActionResult> DeleteAsync([FromRoute] Guid id)
        {
            try
            {
                var entity = await _productRepository.GetByIdAsync(id);

                if (entity == null)
                {
                    throw new Exception("Product not found.");
                }

                await _productRepository.DeleteAsync(id);
                await _productRepository.SaveChangesAsync();

                return Response();
            }
            catch (Exception e)
            {
                return Response(e);
            }
        }
    }
}
