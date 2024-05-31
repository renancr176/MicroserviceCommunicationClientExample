using AuthApi.Models;
using AuthApi.Models.Requests;
using AuthApi.Models.Responses;
using Identity.IdentityDbContext.Entities;

namespace AuthApi.Services;

public interface IUserService : Identity.Services.IUserService
{

    #region For internal purposes

    Task<bool> SendEmailConfirmationAsync(Guid userId);

    #endregion

    #region For external purposes

    Task<SignInResponseModel?> SignInAsync(SignInRequest request);
    Task<UserModel?> SignUpAsync(SignUpRequest request);

    #endregion

    #region Jwt

    Task<SignInResponseModel?> GetJwtAsync(User user);
    Task<SignInResponseModel?> RefreshTokenAsync(string encodedJwt, string refreshToken);
    Task AddRoleAsync(UserAddRoleRequest request);

    #endregion
}