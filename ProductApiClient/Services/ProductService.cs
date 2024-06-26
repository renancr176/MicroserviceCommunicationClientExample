﻿using Flurl.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ProductApiClient.Extensions;
using ProductApiClient.Interfaces.Services;
using ProductApiClient.Models;
using ProductApiClient.Models.Requests;
using ProductApiClient.Models.Responses;
using System.Drawing;

namespace ProductApiClient.Services;

public class ProductService : IProductService
{
    private readonly IAuthService _authService;
    private readonly IForwardAuthService _forwardAuthService;

    public ProductService(IAuthService authService, IForwardAuthService forwardAuthService)
    {
        _authService = authService;
        _forwardAuthService = forwardAuthService;
    }

    private const string Controller = "/Product";

    private IFlurlRequest Url => _forwardAuthService.IsUserAuthenticated
        ? _forwardAuthService.Url.AppendPathSegment(Controller)
        : _authService.Url.AppendPathSegment(Controller);


    public async Task<BaseResponse<IEnumerable<ProductResponseModel>?>> GetPagedAsync(int page = 1, int size = 50)
    {
        try
        {
            return await Url
                .SetQueryParams(new { page, size })
                .ResilientGetJsonAsync<BaseResponse<IEnumerable<ProductResponseModel>?>>();
        }
        catch (FlurlHttpException e)
        {
            return await e.ToResponseAsync<IEnumerable<ProductResponseModel>?>();
        }
        catch (Exception e)
        {
            return e.ToResponse<IEnumerable<ProductResponseModel>?>();
        }
    }

    public async Task<BaseResponse<IEnumerable<ProductResponseModel>?>> SearchAsync(ProductSearchRequest request)
    {
        try
        {
            return await Url
                .AppendPathSegment("/Search")
                .SetJsonQueryParams(request)
                .ResilientGetJsonAsync<BaseResponse<IEnumerable<ProductResponseModel>?>>();
        }
        catch (FlurlHttpException e)
        {
            return await e.ToResponseAsync<IEnumerable<ProductResponseModel>?>();
        }
        catch (Exception e)
        {
            return e.ToResponse<IEnumerable<ProductResponseModel>?>();
        }
    }

    public async Task<BaseResponse<ProductResponseModel?>> GetByIdAsync(Guid id)
    {
        try
        {
            return await Url
                .AppendPathSegment($"/{id}")
                .ResilientGetJsonAsync<BaseResponse<ProductResponseModel?>>();
        }
        catch (FlurlHttpException e)
        {
            return await e.ToResponseAsync<ProductResponseModel?>();
        }
        catch (Exception e)
        {
            return e.ToResponse<ProductResponseModel?>();
        }
    }

    public async Task<BaseResponse<ProductResponseModel?>> CreateAsync(CreateProductRequest request)
    {
        try
        {
            var result = await Url
                .ResilientPostJsonAsync(request);

            return await result.GetJsonAsync<BaseResponse<ProductResponseModel?>>();
        }
        catch (FlurlHttpException e)
        {
            return await e.ToResponseAsync<ProductResponseModel?>();
        }
        catch (Exception e)
        {
            return e.ToResponse<ProductResponseModel?>();
        }
    }

    public async Task<BaseResponse<ProductResponseModel?>> UpdateAsync(UpdateProductRequest request)
    {
        try
        {
            var result = await Url
                .ResilientPutJsonAsync(request);

            return await result.GetJsonAsync<BaseResponse<ProductResponseModel?>>();
        }
        catch (FlurlHttpException e)
        {
            return await e.ToResponseAsync<ProductResponseModel?>();
        }
        catch (Exception e)
        {
            return e.ToResponse<ProductResponseModel?>();
        }
    }

    public async Task<BaseResponse> DeleteAsync(Guid id)
    {
        try
        {
            var result = await Url
                .AppendPathSegment($"/{id}")
                .ResilientDeleteAsync();

            return await result.GetJsonAsync<BaseResponse<ProductResponseModel?>>();
        }
        catch (FlurlHttpException e)
        {
            return await e.ToResponseAsync<ProductResponseModel?>();
        }
        catch (Exception e)
        {
            return e.ToResponse<ProductResponseModel?>();
        }
    }
}