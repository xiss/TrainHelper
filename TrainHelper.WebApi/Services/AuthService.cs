﻿using AutoMapper;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using TrainHelper.DAL.Entities;
using TrainHelper.DAL.Providers;
using TrainHelper.WebApi.Config;
using TrainHelper.WebApi.Constants;
using TrainHelper.WebApi.Dto;
using TrainHelper.WebApi.Dto.Token;

namespace TrainHelper.WebApi.Services;

public class AuthService : IAuthService
{
    private readonly AuthSettings _authSettings;
    private readonly IMapper _mapper;
    private readonly IUserDataProvider _userDataProvider;

    public AuthService(IOptions<AuthSettings> appConfig, IMapper mapper, IUserDataProvider userDataProvider)
    {
        _authSettings = appConfig.Value;
        _mapper = mapper;
        _userDataProvider = userDataProvider;
    }

    /// <summary>
    /// Create new user by dto
    /// </summary>
    /// <param name="dto"></param>
    public async Task CreateUser(CreateUserDto dto)
    {
        var user = _mapper.Map<User>(dto);
        user.PasswordHash = GetHashSha256(dto.Password);
        await _userDataProvider.AddUser(user);
    }

    /// <summary>
    /// Get token by login and password
    /// </summary>
    /// <param name="login"></param>
    /// <param name="password"></param>
    public async Task<TokenResultDto> GetToken(string login, string password)
    {
        var user = await GetUserByCredential(login, password);
        if (user == null) return new TokenResultDto(Error: "User not found or wrong password");
        var session = await _userDataProvider.AddUserSession(new()
        {
            Created = DateTimeOffset.UtcNow,
            User = user,
            RefreshTokenId = Guid.NewGuid(),
        });

        return new TokenResultDto(Token: GenerateTokens(session));
    }

    /// <summary>
    /// Get token by refresh token
    /// </summary>
    /// <param name="refreshToken"></param>
    public async Task<TokenResultDto> GetTokenByRefreshToken(string refreshToken)
    {
        ClaimsPrincipal principal;
        SecurityToken securityToken;
        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateIssuerSigningKey = true,
            ValidateAudience = false,
            ValidateLifetime = true,
            IssuerSigningKey = _authSettings.GetSymmetricSecurityKey()
        };

        try
        {
            principal = new JwtSecurityTokenHandler().ValidateToken(refreshToken, validationParameters, out securityToken);
        }
        catch (Exception e)
        {
            return new TokenResultDto(Error: $"Invalid security token - {e.Message}");
        }

        if (securityToken is not JwtSecurityToken jwtSecurityToken
            || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                StringComparison.InvariantCultureIgnoreCase))
        {
            return new TokenResultDto(Error: "Invalid security token");
        }

        if (principal.Claims.FirstOrDefault(c => c.Type == ClaimNames.RefreshTokenId)?.Value is not string refreshIdString
            || !Guid.TryParse(refreshIdString, out var refreshTokenId))
        {
            return new TokenResultDto(Error: "Invalid refresh token");
        }
        var session = await _userDataProvider.GetUserSessionByRefreshToken(refreshTokenId);
        if (session == null || !session.IsActive)
            return new TokenResultDto(Error: "Session is inactive or not found");

        await _userDataProvider.UpdateRefreshTokenId(session, Guid.NewGuid());

        return new TokenResultDto(Token: GenerateTokens(session));
    }

    /// <summary>
    /// Get user by login and password
    /// </summary>
    /// <param name="login"></param>
    /// <param name="password"></param>
    public async Task<User?> GetUserByCredential(string login, string password)
    {
        var user = await _userDataProvider.GetUserByLogin(login);
        if (user == null)
            return null;

        return !VerifySha256(password, user.PasswordHash) ? null : user;
    }

    private static string GetHashSha256(string input)
    {
        using var sha = SHA256.Create();
        var hash = sha.ComputeHash(Encoding.UTF8.GetBytes(input));
        var sb = new StringBuilder();

        foreach (var item in hash)
        {
            sb.Append(item.ToString("x2"));
        }
        return sb.ToString();
    }

    private static bool VerifySha256(string input, string hash) =>
         hash.Equals(GetHashSha256(input), StringComparison.OrdinalIgnoreCase);

    private TokenDto GenerateTokens(UserSession session)
    {
        var now = DateTime.Now;

        //SecurityToken
        Claim[] claims =
        {
            new(ClaimsIdentity.DefaultNameClaimType,  session.User.Name),
            new(ClaimNames.Login , session.User.Email),
            new(ClaimNames.UserId, session.User.Id.ToString()),
            new(ClaimNames.SessionId, session.Id.ToString())
        };
        var securityToken = new JwtSecurityToken(
            issuer: _authSettings.Issuer,
            audience: _authSettings.Audience,
            notBefore: now,
            claims: claims,
            expires: now.AddMinutes(_authSettings.LifeTime),
            signingCredentials: new SigningCredentials(_authSettings.GetSymmetricSecurityKey(),
                SecurityAlgorithms.HmacSha256));
        var encodedToken = new JwtSecurityTokenHandler().WriteToken(securityToken);

        //RefreshToken
        claims = new Claim[]
        {
            new(ClaimNames.RefreshTokenId, session.RefreshTokenId.ToString()),
        };
        var refreshToken = new JwtSecurityToken(
            notBefore: now,
            claims: claims,
            expires: now.AddHours(_authSettings.LifeTime),
            signingCredentials: new SigningCredentials(_authSettings.GetSymmetricSecurityKey(),
                SecurityAlgorithms.HmacSha256));
        var encodedRefreshToken = new JwtSecurityTokenHandler().WriteToken(refreshToken);

        return new TokenDto(encodedToken, encodedRefreshToken);
    }
}