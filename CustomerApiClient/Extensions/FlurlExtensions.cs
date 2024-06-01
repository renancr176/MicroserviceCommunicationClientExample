using System.Linq.Expressions;
using System.Net;
using CustomerApiClient.Models.Responses;
using Flurl.Http;
using Newtonsoft.Json;
using Polly;
using Polly.Retry;
using NullValueHandling = Flurl.NullValueHandling;

namespace CustomerApiClient.Extensions;

public static class FlurlExtensions
{
    public static IFlurlRequest SetJsonQueryParams(this IFlurlRequest request, object values,
        NullValueHandling nullValueHandling = NullValueHandling.Remove)
    {
        var valueSerialized = JsonConvert.SerializeObject(values);
        values = JsonConvert.DeserializeObject<Dictionary<string, string>>(valueSerialized);
        return request.SetQueryParams(values);
    }

    public static ResiliencePipeline DefaultResiliencePipeline(this IFlurlRequest request)
    {
        return new ResiliencePipelineBuilder()
            .AddRetry(new RetryStrategyOptions()
            {
                BackoffType = DelayBackoffType.Exponential,
                MaxRetryAttempts = 3,
                Delay = TimeSpan.FromSeconds(3),
            })
            .AddTimeout(request.Client.HttpClient.Timeout)
            .Build();
    }

    public static async Task<T> ResilientGetJsonAsync<T>(this IFlurlRequest request,
        HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead,
        CancellationToken cancellationToken = default)
    {
        var pipeline = request.DefaultResiliencePipeline();

        T response = default(T);
        
        await pipeline.ExecuteAsync(async token =>
        {
             response = await request.GetJsonAsync<T>(completionOption, cancellationToken);
        }, cancellationToken);

        return response;
    }

    public static async Task<IFlurlResponse> ResilientPostJsonAsync(this IFlurlRequest request, object body,
        HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead,
        CancellationToken cancellationToken = default)
    {
        var pipeline = request.DefaultResiliencePipeline();

        IFlurlResponse response = default(IFlurlResponse);

        await pipeline.ExecuteAsync(async token =>
        {
            response = await request.PostJsonAsync(body, completionOption, cancellationToken);
        }, cancellationToken);

        return response;
    }

    public static async Task<IFlurlResponse> ResilientPutJsonAsync(this IFlurlRequest request, object body,
        HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead,
        CancellationToken cancellationToken = default)
    {
        var pipeline = request.DefaultResiliencePipeline();

        IFlurlResponse response = default(IFlurlResponse);

        await pipeline.ExecuteAsync(async token =>
        {
            response = await request.PutJsonAsync(body, completionOption, cancellationToken);
        }, cancellationToken);

        return response;
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