using MoodAtlas.Models;
using MoodAtlas.Config;
using SQLite;

namespace MoodAtlas.Services;

public class UserService
{
    private readonly SQLiteAsyncConnection _db;

    public UserService()
    {
        _db = DatabaseConfig.GetConnection();
    }

    public async Task<int> CreateUserAsync(User user)
    {
        user.CreatedAt = DateTime.UtcNow;
        user.UpdatedAt = DateTime.UtcNow;
        return await _db.InsertAsync(user);
    }

    public async Task<User> GetUserByIdAsync(int userId)
    {
        return await _db.GetAsync<User>(userId);
    }

    public async Task<List<User>> GetAllUsersAsync()
    {
        return await _db.Table<User>().ToListAsync();
    }

    public async Task<int> UpdateUserAsync(User user)
    {
        user.UpdatedAt = DateTime.UtcNow;
        return await _db.UpdateAsync(user);
    }

    public async Task<int> DeleteUserAsync(int userId)
    {
        return await _db.DeleteAsync<User>(userId);
    }
}
