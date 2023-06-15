using Microsoft.AspNetCore.Mvc;
using TrainHelper.WebApi.Dto;
using TrainHelper.WebApi.Dto.Token;
using TrainHelper.WebApi.Services.Interfaces;

namespace TrainHelper.WebApi.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class AuthController : Controller
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService) => _authService = authService;

    [HttpPost]
    public async Task<TokenResultDto> RefreshToken(RefreshTokenRequestDto dto)
        => await _authService.GetTokenByRefreshToken(dto.RefreshToken);

    [HttpPost]
    public async Task RegisterUser(CreateUserDto dto)
        => await _authService.CreateUser(dto);

    [HttpPost]
    public async Task<TokenResultDto> Token(TokenRequestDto dto)
        => await _authService.GetToken(dto.Login, dto.Password);
}