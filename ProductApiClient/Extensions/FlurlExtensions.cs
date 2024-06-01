using Flurl.Http;
using Newtonsoft.Json;
using System.Net;
using ProductApiClient.Models.Responses;
using NullValueHandling = Flurl.NullValueHandling;

namespace ProductApiClient.Extensions;

public static class FlurlExtensions
{
    public static IFlurlRequest SetJsonQueryParams(this IFlurlRequest request, object values,
        NullValueHandling nullValueHandling = NullValueHandling.Remove)
    {
        var valueSerialized = JsonConvert.SerializeObject(values);
        values = JsonConvert.DeserializeObject<Dictionary<string, string>>(valueSerialized);
        return request.SetQueryParams(values);
    }

    public static async Task<BaseResponse> ToResponseAsync(this FlurlHttpException e)
    {
        return new BaseResponse()
        {
            Errors = new List<BaseResponseError>()
            {
                new BaseResponseError()
                {
                    ErrorCode = e?.Call?.HttpResponseMessage?.StatusCode != null
                        ? ((HttpStatusCode)e.Call.HttpResponseMessage.StatusCode).ToString()
                        : "Exception",
                    Message = e?.Call?.HttpResponseMessage?.Content != null
                        ? await e.Call.HttpResponseMessage.Content.ReadAsStringAsync()
                        : e.Message,
                }
            }
        };
    }

    public static BaseResponse ToResponse(this Exception e)
    {
        return new BaseResponse()
        {
            Errors = new List<BaseResponseError>()
            {
                new BaseResponseError()
                {
                    ErrorCode = "Exception",
                    Message = e.Message,
                }
            }
        };
    }

    public static async Task<BaseResponse<TResponse>> ToResponseAsync<TResponse>(this FlurlHttpException e)
    {
        return new BaseResponse<TResponse>()
        {
            Errors = new List<BaseResponseError>()
            {
                new BaseResponseError()
                {
                    ErrorCode = e?.Call?.HttpResponseMessage?.StatusCode != null
                        ? ((HttpStatusCode)e.Call.HttpResponseMessage.StatusCode).ToString()
                        : "Exception",
                    Message = e?.Call?.HttpResponseMessage?.Content != null
                        ? await e.Call.HttpResponseMessage.Content.ReadAsStringAsync()
                        : e.Message,
                }
            }
        };
    }

    public static BaseResponse<TResponse> ToResponse<TResponse>(this Exception e)
    {
        return new BaseResponse<TResponse>()
        {
            Errors = new List<BaseResponseError>()
            {
                new BaseResponseError()
                {
                    ErrorCode = "Exception",
                    Message = e.Message,
                }
            }
        };
    }
}