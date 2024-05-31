using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using AuthApi.Models;
using AuthApi.Models.Requests;
using AuthApi.Models.Responses;
using DomainCore.Extensions;
using DomainCore.Options;
using Identity.IdentityDbContext.Entities;
using Identity.IdentityDbContext.Interfaces.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace AuthApi.Services;

public class UserService : Identity.Services.UserService, IUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;
    private readonly IOptions<JwtTokenOptions> _jwtTokenOptions;
    private readonly IRefreshTokenRepository _refreshTokenRepository;

    public UserService(IHttpContextAccessor httpContextAccessor, UserManager<User> userManager,
        SignInManager<User> signInManager, RoleManager<IdentityRole<Guid>> roleManager,
        IOptions<JwtTokenOptions> jwtTokenOptions, IRefreshTokenRepository refreshTokenRepository) : base(
        httpContextAccessor, userManager)
    {
        _httpContextAccessor = httpContextAccessor;
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
        _jwtTokenOptions = jwtTokenOptions;
        _refreshTokenRepository = refreshTokenRepository;
    }

    #region Consts

    public const string PasswordRole = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.*[!@#$%¨&*_+-=^~?<>]).{8,50}$";

    public const string EmailConfirmationSubject = "Email confirmation";
    public const string EmailConfirmationBody = @"<p>Hello #Name</p>
<br/>
<p>To confirm your email, follow the above steps.</p>
<br/>
<h3 style=""text-align:center"">#EmailConfirmationToken</h3>";

    #endregion

    #region For internal purposes

    public async Task<bool> SendEmailConfirmationAsync(Guid userId)
    {
        try
        {
            throw new NotImplementedException();

            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user == null || user.EmailConfirmed)
            {
                return false;
            }

            var body = EmailConfirmationBody
                .Replace("#Name", user.Name)
                .Replace("#EmailConfirmationToken", user.EmailConfirmationToken);

            //return await _mailService.SendAsync(new SendMailResquest(user.Email, EmailConfirmationSubject, body));
        }
        catch (Exception e)
        {
        }

        return false;
    }

    #endregion

    #region For external purposes

    public async Task<SignInResponseModel?> SignInAsync(SignInRequest request)
    {
        var result = await _signInManager.PasswordSignInAsync(request.UserName, request.Password, false, true);

        if (!result.Succeeded)
        {
            if (result.IsLockedOut)
            {
                throw new Exception("Login is locked, try again later.");
            }
            else
            {
                throw new Exception("The user name or password is invalid.");
            }
        }

        var user = await _userManager.FindByNameAsync(request.UserName);

        //if (!user.EmailConfirmed)
        //{
        //    await SendEmailConfirmationAsync(user.Id);
        //    throw new Exception("Email not confirmed yet.");
        //}

        return await GetJwtAsync(user);
    }

    public async Task<UserModel?> SignUpAsync(SignUpRequest request)
    {
        var user = new User(request.UserName, request.Name, request.Email, request.RememberPhrase);

        if (!user.Email.IsValidEmail())
        {
            throw new Exception("");
        }

        if (string.IsNullOrEmpty(request.Password)
        || !Regex.IsMatch(request.Password, PasswordRole))
        {

            throw new Exception("");
        }

        var result = await _userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            throw new Exception("Unable to create new user.", new Exception(result.Errors.FirstOrDefault()?.Description));
        }

        //await SendEmailConfirmationAsync(user.Id);

        return new UserModel()
        {
            Id = user.Id,
            UserName = user.UserName,
            Name = user.Name,
            Email = user.Email,
            RememberPhrase = user.RememberPhrase
        };
    }

    public async Task AddRoleAsync(UserAddRoleRequest request)
    {
        var user = await _userManager.FindByIdAsync(request.UserId.ToString());

        if (user == null)
        {
            throw new Exception("User not found.");
        }

        foreach (var role in request.Roles)
        {
            if (!await _roleManager.RoleExistsAsync(role.ToString()))
            {
                throw new Exception(@$"Role ""{role}"" not found.");
            }
        }

        foreach (var role in request.Roles)
        {
            await _userManager.AddToRoleAsync(user, role.ToString());
        }
    }

    #endregion

    #region Jwt

    private static long ToUnixEpochDate(DateTime date)
        => (long)Math.Round((date.ToUniversalTime() - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero)).TotalSeconds);

    private RefreshToken GenerateRefreshToken(Guid userId)
    {
        var randomNumber = new byte[new Random().Next(32, 256)];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return new RefreshToken(
            userId,
            Convert.ToBase64String(randomNumber),
            _jwtTokenOptions.Value.RefreshTokenValidUntil);
    }

    private async Task<ClaimsPrincipal?> GetPrincipalFromExpiredTokenAsync(string encodedJwt)
    {
        var tokenValidationParameters = new TokenValidationParameters()
        {
            ValidateAudience = false,
            ValidateIssuer = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = _jwtTokenOptions.Value.IssuerSigningKey,
            ValidateLifetime = false
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(encodedJwt, tokenValidationParameters, out var securityToken);
        if (securityToken is not JwtSecurityToken jwtSecurityToken
            || !jwtSecurityToken.Header.Alg.Equals(_jwtTokenOptions.Value.JwtSecurityAlgorithms,
                StringComparison.InvariantCultureIgnoreCase))
        {
            throw new Exception("Invalid JWT token");
        }

        return principal;
    }

    public async Task<SignInResponseModel?> GetJwtAsync(User user)
    {
        var userClaims = await _userManager.GetClaimsAsync(user);

        userClaims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
        userClaims.Add(new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()));
        userClaims.Add(new Claim(JwtRegisteredClaimNames.Email, user.Email));
        userClaims.Add(new Claim(JwtRegisteredClaimNames.Jti, await _jwtTokenOptions.Value.JtiGenerator()));
        userClaims.Add(new Claim(JwtRegisteredClaimNames.Iat, ToUnixEpochDate(_jwtTokenOptions.Value.IssuedAt).ToString(), ClaimValueTypes.Integer64));

        var roles = await _userManager.GetRolesAsync(user);
        foreach (var role in roles)
        {
            userClaims.Add(new Claim(ClaimsIdentity.DefaultRoleClaimType, role));
            userClaims.Add(new Claim("roles", role));
        }

        var jwt = new JwtSecurityToken(
            issuer: _jwtTokenOptions.Value.Issuer,
            audience: _jwtTokenOptions.Value.Audience,
            claims: userClaims,
            notBefore: _jwtTokenOptions.Value.NotBefore,
            expires: _jwtTokenOptions.Value.Expiration,
            signingCredentials: _jwtTokenOptions.Value.SigningCredentials);

        var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

        var refreshToken = GenerateRefreshToken(user.Id);

        var retry = true;
        do
        {
            if (await _refreshTokenRepository.AnyAsync(x => x.Token == refreshToken.Token))
            {
                refreshToken = GenerateRefreshToken(user.Id);
            }
            else
            {
                retry = false;
            }
        } while (retry);


        await _refreshTokenRepository.InsertAsync(refreshToken);
        await _refreshTokenRepository.SaveChangesAsync();

        return new SignInResponseModel(
            encodedJwt,
            _jwtTokenOptions.Value.ValidFor.TotalSeconds,
            refreshToken.Token,
            _jwtTokenOptions.Value.RefreshTokenValidForMore.TotalSeconds,
            new UserModel()
            {
                Id = user.Id,
                Name = user.Name,
                UserName = user.UserName,
                Email = user.Email,
                RememberPhrase = user.RememberPhrase,
                Roles = roles.ToList()
            });
    }

    public async Task<SignInResponseModel?> RefreshTokenAsync(string encodedJwt, string refreshToken)
    {
        var userId = Guid.Empty;
        try
        {
            var tokenValidationParameters = new TokenValidationParameters()
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = _jwtTokenOptions.Value.IssuerSigningKey,
                ValidateLifetime = true
            };
            var tokenHandler = new JwtSecurityTokenHandler();

            if (tokenHandler.ValidateToken(encodedJwt, tokenValidationParameters, out var securityToken) != null)
            {
                throw new Exception("JWT token is invalid");
            }

            var principal = await GetPrincipalFromExpiredTokenAsync(encodedJwt);
            Guid.TryParse(principal.FindFirstValue(ClaimTypes.NameIdentifier), out userId);
            if (userId == null || userId == Guid.Empty)
            {
                return default;
            }

            if (!await _refreshTokenRepository.AnyAsync(e =>
                    e.UserId == userId
                    && e.Token == refreshToken
                    && e.ValidUntil > DateTime.UtcNow))
            {
                throw new Exception("Invalid JWT refresh token");
            }

            var user = await _userManager.FindByIdAsync(userId.ToString());
            return await GetJwtAsync(user);
        }
        finally
        {
            if (userId != Guid.Empty
            && await _refreshTokenRepository.AnyAsync(e =>
                e.UserId == userId
                && e.Token == refreshToken))
            {
                await _refreshTokenRepository.DeleteAsync(e =>
                    e.UserId == userId
                    && e.Token == refreshToken);

                await _refreshTokenRepository.SaveChangesAsync();
            }
        }
    }

    #endregion
}