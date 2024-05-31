using DomainCore.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AuthApi.Controllers;

public abstract class BaseController : Controller
{
    protected Guid ClienteId;

    public BaseController(IHttpContextAccessor httpContextAccessor)
    {
        if (httpContextAccessor.HttpContext.User != null)
        {
            var claim = httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
            if (claim != null)
                ClienteId = Guid.Parse(claim.Value);
        }
    }

    protected new IActionResult Response()
    {
        return Ok(new BaseResponse());
    }

    protected new IActionResult Response(object? result = null)
    {
        return Ok(new BaseResponse<object?>()
        {
            Data = result
        });
    }

    protected new IActionResult Response(Exception e)
    {
        return BadRequest(new BaseResponse<object>()
        {
            Errors = new List<BaseResponseError>() { new BaseResponseError() { ErrorCode = "", Message = e.Message } }
        });
    }

    protected IActionResult InvalidModelResponse()
    {
        return BadRequest(new BaseResponse()
        {
            Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => new BaseResponseError()
            {
                ErrorCode = "ModelError",
                Message = e.ErrorMessage
            }).ToList()
        });
    }
}