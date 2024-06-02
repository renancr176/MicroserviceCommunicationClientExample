using System.Collections;
using System.Net;
using CustomerApiClient.Models.Responses;
using Flurl.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Polly;
using Polly.Retry;
using NullValueHandling = Flurl.NullValueHandling;

namespace CustomerApiClient.Extensions;

public static class FlurlExtensions
{
    public static IFlurlRequest SetJsonQueryParams(this IFlurlRequest request, object values,
        NullValueHandling nullValueHandling = NullValueHandling.Remove)
    {
        var properties = values.GetType().GetProperties().ToList();
        var jsonObject = JObject.Parse(JsonConvert.SerializeObject(values));

        var queryParams = new List<KeyValuePair<string, object>>();

        foreach (var property in properties.Where(p => p.GetValue(values) != null))
        {
            var jsonPropName = jsonObject.Properties().ToArray()[properties.IndexOf(property)].Name;
            if (property.PropertyType.Name.Contains("List")
                || property.PropertyType.Name.Contains("IList")
                || property.PropertyType.Name.Contains("IEnumerable")
                || property.PropertyType.Name.Contains("ICollection")
                || property.PropertyType.Name.Contains("Array"))
            {
                foreach (var val in (IEnumerable)property.GetValue(values))
                {
                    queryParams.Add(new KeyValuePair<string, object>(jsonPropName, val));
                }
            }
            else
            {
                queryParams.Add(new KeyValuePair<string, object>(jsonPropName, property.GetValue(values)));
            }
        }

        return request.SetQueryParams(queryParams, nullValueHandling);
    }

    public static ResiliencePipeline DefaultResiliencePipeline(this IFlurlRequest request)
    {
        return new ResiliencePipelineBuilder()
            .AddRetry(new RetryStrategyOptions()
            {
                ShouldHandle = new PredicateBuilder().Handle<FlurlHttpTimeoutException>(),
                BackoffType = DelayBackoffType.Exponential,
                MaxRetryAttempts = 3,
                Delay = TimeSpan.FromSeconds(3),
            })
            .AddTimeout(request.Settings.Timeout ?? TimeSpan.FromSeconds(100))
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

    public static async Task<IFlurlResponse> ResilientDeleteAsync(this IFlurlRequest request,
        HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead,
        CancellationToken cancellationToken = default)
    {
        var pipeline = request.DefaultResiliencePipeline();

        IFlurlResponse response = default(IFlurlResponse);

        await pipeline.ExecuteAsync(async token =>
        {
            response = await request.DeleteAsync(completionOption, cancellationToken);
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