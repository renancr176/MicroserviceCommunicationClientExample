using AuthApi.Models.Responses;
using AuthApi.Models;
using AuthApi.Models.Requests;
using AuthApi.Services;
using DomainCore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace AuthApi.Controllers ;

[Route("[controller]")]
[ApiController]
public class UserController : BaseController
{
    private readonly IUserService _userService;

    public UserController(IHttpContextAccessor httpContextAccessor, IUserService userService) 
        : base(httpContextAccessor)
    {
        _userService = userService;
    }

    [HttpPost("SignIn")]
    [AllowAnonymous]
    [SwaggerResponse(200, Type = typeof(BaseResponse<SignInResponseModel?>))]
    [SwaggerResponse(400, Type = typeof(BaseResponse))]
    public async Task<IActionResult> SignInAsync([FromBody] SignInRequest request)
    {
        if (!ModelState.IsValid) return InvalidModelResponse();

        try
        {
            return Response(await _userService.SignInAsync(request));
        }
        catch (Exception e)
        {
            return Response(e);
        }
    }

    [HttpPost("SignIn/Refresh")]
    [AllowAnonymous]
    [SwaggerResponse(200, Type = typeof(BaseResponse<SignInResponseModel?>))]
    [SwaggerResponse(400, Type = typeof(BaseResponse))]
    public async Task<IActionResult> SignInRefreshAsync([FromBody] SignInRefreshRequest request)
    {
        if (!ModelState.IsValid) return InvalidModelResponse();

        try
        {
            return Response(await _userService.RefreshTokenAsync(request.AccessToken, request.RefreshToken));
        }
        catch (Exception e)
        {
            return Response(e);
        }
    }

    [HttpPost("SignUp")]
    [AllowAnonymous]
    [SwaggerResponse(200, Type = typeof(BaseResponse<UserModel>))]
    [SwaggerResponse(400, Type = typeof(BaseResponse))]
    public async Task<IActionResult> SignUpAsync([FromBody] SignUpRequest request)
    {
        if (!ModelState.IsValid) return InvalidModelResponse();

        try
        {
            return Response(await _userService.SignUpAsync(request));
        }
        catch (Exception e)
        {
            return Response(e);
        }
    }

    [HttpPost("AddRole")]
    [SwaggerResponse(200, Type = typeof(BaseResponse<UserModel>))]
    [SwaggerResponse(400, Type = typeof(BaseResponse))]
#if !DEBUG
[Authorize("Bearer", Roles = $"{nameof(RoleEnum.Admin)}")]
[ApiExplorerSettings(IgnoreApi = true)]
#else
    [AllowAnonymous]
    [ApiExplorerSettings(IgnoreApi = false)]
#endif
    public async Task<IActionResult> AddRoleAsync([FromBody] UserAddRoleRequest request)
    {
        if (!ModelState.IsValid) return InvalidModelResponse();

        try
        {
            await _userService.AddRoleAsync(request);
            return Response();
        }
        catch (Exception e)
        {
            return Response(e);
        }
    }
}