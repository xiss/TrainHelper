using TrainHelper.DAL.Entities;

namespace TrainHelper.DAL.Providers
{
    public interface IUserDataProvider
    {
        Task<User> AddUser(User user);
        Task<UserSession> AddUserSession(UserSession userSession);
        void Dispose();
        Task<User?> GetUserByLogin(string login);
        Task<UserSession?> GetUserSessionById(int id);
        Task<UserSession?> GetUserSessionByRefreshToken(Guid refreshTokenId);
        Task UpdateRefreshTokenId(UserSession userSession, Guid refreshTokenId);
    }
}