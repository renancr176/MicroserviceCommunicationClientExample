using CustomerApiClient.Models;
using CustomerApiClient.Models.Requests;
using CustomerApiClient.Models.Responses;

namespace CustomerApiClient.Interfaces.Services;

public interface ICustomerService
{
    Task<BaseResponse<IEnumerable<CustomerResponseModel>?>> GetPagedAsync(int page, int size);
    Task<BaseResponse<CustomerResponseModel?>> GetByIdAsync(Guid id);
    Task<BaseResponse<CustomerResponseModel?>> CreateAsync(CreateCustomerRequest request);
    Task<BaseResponse<CustomerResponseModel?>> UpdateAsync(UpdateCustomerRequest request);
}