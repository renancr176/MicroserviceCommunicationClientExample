using DomainCore.Models;
using Identity.Services;
using Microsoft.AspNetCore.Mvc;

namespace OrderApi.Controllers;

public abstract class BaseController : Controller
{
    protected readonly IUserService _userService;

    protected BaseController(IUserService userService)
    {
        _userService = userService;
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