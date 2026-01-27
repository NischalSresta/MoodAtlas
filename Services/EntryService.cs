using MoodAtlas.Models;
using MoodAtlas.Config;
using SQLite;
using Entry = MoodAtlas.Models.Entry;

namespace MoodAtlas.Services;

public class EntryService
{
    private readonly SQLiteAsyncConnection _db;

    public EntryService()
    {
        _db = DatabaseConfig.GetConnection();
    }

    public async Task<int> CreateEntryAsync(Entry entry)
    {
        entry.CreatedAt = DateTime.UtcNow;
        entry.UpdatedAt = DateTime.UtcNow;
        return await _db.InsertAsync(entry);
    }

    public async Task<Entry> GetEntryByIdAsync(int entryId)
    {
        return await _db.GetAsync<Entry>(entryId);
    }

    public async Task<List<Entry>> GetAllEntriesAsync()
    {
        return await _db.Table<Entry>().ToListAsync();
    }

    public async Task<List<Entry>> GetEntriesByUserIdAsync(int userId)
    {
        return await _db.Table<Entry>()
            .Where(e => e.UserId == userId)
            .OrderByDescending(e => e.EntryDate)
            .ToListAsync();
    }

    public async Task<List<Entry>> GetEntriesByCategoryIdAsync(int categoryId)
    {
        return await _db.Table<Entry>()
            .Where(e => e.CategoryId == categoryId)
            .OrderByDescending(e => e.EntryDate)
            .ToListAsync();
    }

    public async Task<Entry> GetEntryByUserAndDateAsync(int userId, DateTime entryDate)
    {
        var dateOnly = entryDate.Date;
        return await _db.Table<Entry>()
            .FirstOrDefaultAsync(e => e.UserId == userId && e.EntryDate.Date == dateOnly);
    }

    public async Task<int> UpdateEntryAsync(Entry entry)
    {
        entry.UpdatedAt = DateTime.UtcNow;
        return await _db.UpdateAsync(entry);
    }

    public async Task<int> DeleteEntryAsync(int entryId)
    {
        return await _db.DeleteAsync<Entry>(entryId);
    }

    public async Task<int> GetTotalEntriesCountAsync(int userId)
    {
        return await _db.Table<Entry>()
            .Where(e => e.UserId == userId)
            .CountAsync();
    }

    public async Task<List<Entry>> GetEntriesByDateRangeAsync(int userId, DateTime startDate, DateTime endDate)
    {
        // Normalize dates to ensure proper comparison
        var start = startDate.Date;
        var end = endDate.Date.AddDays(1).AddTicks(-1); // End of the day
        
        return await _db.Table<Entry>()
            .Where(e => e.UserId == userId && 
                   e.EntryDate >= start && 
                   e.EntryDate <= end)
            .OrderByDescending(e => e.EntryDate)
            .ToListAsync();
    }
}
