using CustomerApiClient.Extensions;
using CustomerApiClient.Interfaces.Services;
using CustomerApiClient.Models;
using CustomerApiClient.Models.Requests;
using CustomerApiClient.Models.Responses;
using Flurl.Http;

namespace CustomerApiClient.Services;

public class CustomerService : ICustomerService
{
    private readonly IAuthService _authService;
    private readonly IForwardAuthService _forwardAuthService;

    public CustomerService(IAuthService authService, IForwardAuthService forwardAuthService)
    {
        _authService = authService;
        _forwardAuthService = forwardAuthService;
    }

    private const string Controller = "/Customer";

    private IFlurlRequest Url => _forwardAuthService.IsUserAuthenticated
        ? _forwardAuthService.Url.AppendPathSegment(Controller)
        : _authService.Url.AppendPathSegment(Controller);

    public async Task<BaseResponse<IEnumerable<CustomerResponseModel>?>> GetPagedAsync(int page, int size)
    {
        try
        {
            return await Url
                .SetQueryParams(new { page, size })
                .ResilientGetJsonAsync<BaseResponse<IEnumerable<CustomerResponseModel>?>>();
        }
        catch (FlurlHttpException e)
        {
            return await e.ToResponseAsync<IEnumerable<CustomerResponseModel>?>();
        }
        catch (Exception e)
        {
            return e.ToResponse<IEnumerable<CustomerResponseModel>?>();
        }
    }

    public async Task<BaseResponse<CustomerResponseModel?>> GetByIdAsync(Guid id)
    {
        try
        {
            return await Url
                .AppendPathSegment($"/{id}")
                .ResilientGetJsonAsync<BaseResponse<CustomerResponseModel?>>();
        }
        catch (FlurlHttpException e)
        {
            return await e.ToResponseAsync<CustomerResponseModel?>();
        }
        catch (Exception e)
        {
            return e.ToResponse<CustomerResponseModel?>();
        }
    }

    public async Task<BaseResponse<CustomerResponseModel?>> CreateAsync(CreateCustomerRequest request)
    {
        try
        {
            var result = await Url
                .ResilientPostJsonAsync(request);

            return await result.GetJsonAsync<BaseResponse<CustomerResponseModel?>>();
        }
        catch (FlurlHttpException e)
        {
            return await e.ToResponseAsync<CustomerResponseModel?>();
        }
        catch (Exception e)
        {
            return e.ToResponse<CustomerResponseModel?>();
        }
    }

    public async Task<BaseResponse<CustomerResponseModel?>> UpdateAsync(UpdateCustomerRequest request)
    {
        try
        {
            var result = await Url
                .ResilientPutJsonAsync(request);

            return await result.GetJsonAsync<BaseResponse<CustomerResponseModel?>>();
        }
        catch (FlurlHttpException e)
        {
            return await e.ToResponseAsync<CustomerResponseModel?>();
        }
        catch (Exception e)
        {
            return e.ToResponse<CustomerResponseModel?>();
        }
    }
}