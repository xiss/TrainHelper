using Microsoft.EntityFrameworkCore;
using TrainHelper.DAL.Entities;

namespace TrainHelper.DAL.Providers;

public class UserDataProvider : IDisposable, IUserDataProvider
{
    private readonly DataContext _dataContext;

    public UserDataProvider(DataContext dataContext) => _dataContext = dataContext;

    public async Task<User> AddUser(User user)
    {
        var addedUser = await _dataContext.Users.AddAsync(user);
        await _dataContext.SaveChangesAsync();
        return addedUser.Entity;
    }

    public async Task<UserSession> AddUserSession(UserSession userSession)
    {
        var addedUserSession = await _dataContext.UserSessions.AddAsync(userSession);
        await _dataContext.SaveChangesAsync();
        return addedUserSession.Entity;
    }

    public void Dispose() => _dataContext.Dispose();

    public async Task<User?> GetUserByLogin(string login)
        => await _dataContext.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == login.ToLower());

    public async Task<UserSession?> GetUserSessionById(int id)
        => await _dataContext.UserSessions.FirstOrDefaultAsync(s => s.Id == id);

    public async Task<UserSession?> GetUserSessionByRefreshToken(Guid refreshTokenId) =>
        await _dataContext.UserSessions
            .Include(s => s.User)
            .FirstOrDefaultAsync(s => s.RefreshTokenId == refreshTokenId);

    public async Task UpdateRefreshTokenId(UserSession userSession, Guid refreshTokenId)
    {
        userSession.RefreshTokenId = refreshTokenId;
        _dataContext.Update(userSession);
        await _dataContext.SaveChangesAsync();
    }
}
