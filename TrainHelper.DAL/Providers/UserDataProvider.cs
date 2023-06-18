using Microsoft.EntityFrameworkCore;
using TrainHelper.DAL.Entities;

namespace TrainHelper.DAL.Providers;

public class UserDataProvider : IDisposable, IUserDataProvider
{
    private readonly DataContext _dataContext;

    public UserDataProvider(DataContext dataContext) => _dataContext = dataContext;

    /// <summary>
    /// Add new user in database
    /// </summary>
    /// <param name="user"></param>
    public async Task<User> AddUser(User user)
    {
        var addedUser = await _dataContext.Users.AddAsync(user);
        await _dataContext.SaveChangesAsync();
        return addedUser.Entity;
    }

    /// <summary>
    /// Add new session in database
    /// </summary>
    /// <param name="userSession"></param>
    public async Task<UserSession> AddUserSession(UserSession userSession)
    {
        var addedUserSession = await _dataContext.UserSessions.AddAsync(userSession);
        await _dataContext.SaveChangesAsync();
        return addedUserSession.Entity;
    }

    public void Dispose() => _dataContext.Dispose();

    /// <summary>
    /// Get user by login
    /// </summary>
    /// <param name="login"></param>
    public async Task<User?> GetUserByLogin(string login)
        => await _dataContext.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == login.ToLower());

    /// <summary>
    /// Get userSession by sessionId
    /// </summary>
    /// <param name="id"></param>
    public async Task<UserSession?> GetUserSessionById(int id)
        => await _dataContext.UserSessions.FirstOrDefaultAsync(s => s.Id == id);

    /// <summary>
    /// Get userSession by refresh token
    /// </summary>
    /// <param name="refreshTokenId"></param>
    public async Task<UserSession?> GetUserSessionByRefreshToken(Guid refreshTokenId) =>
        await _dataContext.UserSessions
            .Include(s => s.User)
            .FirstOrDefaultAsync(s => s.RefreshTokenId == refreshTokenId);

    /// <summary>
    /// Update refreshTokenId for session
    /// </summary>
    /// <param name="userSession"></param>
    /// <param name="refreshTokenId"></param>
    public async Task UpdateRefreshTokenId(UserSession userSession, Guid refreshTokenId)
    {
        userSession.RefreshTokenId = refreshTokenId;
        _dataContext.Update(userSession);
        await _dataContext.SaveChangesAsync();
    }
}