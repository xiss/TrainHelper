using Microsoft.AspNetCore.Mvc;
using TrainHelper.WebApi.Dto;
using TrainHelper.WebApi.Dto.Token;
using TrainHelper.WebApi.Services;

namespace TrainHelper.WebApi.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class AuthController : Controller
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService) => _authService = authService;

    /// <summary>
    /// Get token by refresh token
    /// </summary>
    /// <param name="dto"></param>
    [HttpPost]
    public async Task<ActionResult> RefreshToken(RefreshTokenRequestDto dto)
    {
        var result = await _authService.GetTokenByRefreshToken(dto.RefreshToken);
        return result.Error != null ? Unauthorized(result.Error) : new JsonResult(result);
    }

    /// <summary>
    /// Register new user
    /// </summary>
    /// <param name="dto"></param>
    [HttpPost]
    public async Task RegisterUser(CreateUserDto dto)
        => await _authService.CreateUser(dto);

    /// <summary>
    /// Get token by login and password
    /// </summary>
    /// <param name="dto"></param>
    [HttpPost]
    public async Task<ActionResult> Token(TokenRequestDto dto)
    {
        var result = await _authService.GetToken(dto.Login, dto.Password);
        return result.Error != null ? Unauthorized(result.Error) : new JsonResult(result);
    }
}