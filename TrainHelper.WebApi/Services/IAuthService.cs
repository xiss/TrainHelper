using TrainHelper.DAL.Entities;
using TrainHelper.WebApi.Dto;
using TrainHelper.WebApi.Dto.Token;

namespace TrainHelper.WebApi.Services;

public interface IAuthService
{
    Task CreateUser(CreateUserDto dto);
    Task<TokenResultDto> GetToken(string login, string password);
    Task<TokenResultDto> GetTokenByRefreshToken(string refreshToken);
    Task<User?> GetUserByCredential(string login, string password);
}