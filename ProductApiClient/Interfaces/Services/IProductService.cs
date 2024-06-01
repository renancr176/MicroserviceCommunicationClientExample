using ProductApiClient.Models;
using ProductApiClient.Models.Requests;
using ProductApiClient.Models.Responses;

namespace ProductApiClient.Interfaces.Services;

public interface IProductService
{
    Task<BaseResponse<IEnumerable<ProductResponseModel>?>> GetPagedAsync(int page = 1, int size = 50);
    Task<BaseResponse<ProductResponseModel?>> GetByIdAsync(Guid id);
    Task<BaseResponse<ProductResponseModel?>> CreateAsync(CreateProductRequest request);
    Task<BaseResponse<ProductResponseModel?>> UpdateAsync(UpdateProductRequest request);
    Task<BaseResponse> DeleteAsync(Guid id);
}