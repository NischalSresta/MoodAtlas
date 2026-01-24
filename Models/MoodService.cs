using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MoodAtlas.Models;
using MoodAtlas.Config;
using SQLite;

namespace MoodAtlas.Services;

public class MoodService
{
    private readonly SQLiteAsyncConnection _db;

    public MoodService()
    {
        _db = DatabaseConfig.GetConnection();
    }

    public async Task<int> CreateMoodAsync(Mood mood)
    {
        mood.CreatedAt = DateTime.UtcNow;
        mood.UpdatedAt = DateTime.UtcNow;
        return await _db.InsertAsync(mood);
    }

    public async Task<Mood> GetMoodByIdAsync(int moodId)
    {
        return await _db.GetAsync<Mood>(moodId);
    }

    public async Task<List<Mood>> GetAllMoodsAsync()
    {
        return await _db.Table<Mood>().OrderBy(m => m.Category).ThenBy(m => m.Name).ToListAsync();
    }

    public async Task<List<Mood>> GetMoodsByCategoryAsync(string category)
    {
        return await _db.Table<Mood>()
            .Where(m => m.Category == category)
            .OrderBy(m => m.Name)
            .ToListAsync();
    }

    public async Task<List<Mood>> GetDefaultMoodsAsync()
    {
        return await _db.Table<Mood>()
            .Where(m => m.IsDefault)
            .OrderBy(m => m.Category).ThenBy(m => m.Name)
            .ToListAsync();
    }

    public async Task<List<Mood>> GetMoodsForEntryAsync(int entryId)
    {
        var entryMoods = await _db.Table<EntryMood>()
            .Where(em => em.EntryId == entryId)
            .ToListAsync();

        var moods = new List<Mood>();
        foreach (var entryMood in entryMoods)
        {
            var mood = await _db.GetAsync<Mood>(entryMood.MoodId);
            if (mood != null)
            {
                moods.Add(mood);
            }
        }
        return moods;
    }

    public async Task<int> AddMoodToEntryAsync(int entryId, int moodId, bool isPrimary = false)
    {
        var entryMood = new EntryMood
        {
            EntryId = entryId,
            MoodId = moodId,
            IsPrimary = isPrimary,
            CreatedAt = DateTime.UtcNow
        };
        return await _db.InsertAsync(entryMood);
    }

    public async Task<int> RemoveMoodFromEntryAsync(int entryId, int moodId)
    {
        return await _db.Table<EntryMood>()
            .Where(em => em.EntryId == entryId && em.MoodId == moodId)
            .DeleteAsync();
    }

    public async Task<int> SetPrimaryMoodForEntryAsync(int entryId, int moodId)
    {
        // Get all moods for this entry and set them to non-primary
        var entryMoods = await _db.Table<EntryMood>()
            .Where(em => em.EntryId == entryId)
            .ToListAsync();

        foreach (var em in entryMoods)
        {
            em.IsPrimary = false;
            await _db.UpdateAsync(em);
        }

        // Set new primary mood
        var entryMood = await _db.Table<EntryMood>()
            .FirstOrDefaultAsync(em => em.EntryId == entryId && em.MoodId == moodId);

        if (entryMood != null)
        {
            entryMood.IsPrimary = true;
            return await _db.UpdateAsync(entryMood);
        }
        return 0;
    }

    public async Task<int> UpdateMoodAsync(Mood mood)
    {
        mood.UpdatedAt = DateTime.UtcNow;
        return await _db.UpdateAsync(mood);
    }

    public async Task<int> DeleteMoodAsync(int moodId)
    {
        // Delete from EntryMoods first
        await _db.Table<EntryMood>().DeleteAsync(em => em.MoodId == moodId);
        return await _db.DeleteAsync<Mood>(moodId);
    }

    public async Task<Dictionary<string, int>> GetMoodDistributionAsync(int userId, DateTime? startDate = null, DateTime? endDate = null)
    {
        var distribution = new Dictionary<string, int>
        {
            ["Positive"] = 0,
            ["Neutral"] = 0,
            ["Negative"] = 0
        };

        // Get entries for user
        var entries = await _db.Table<Models.Entry>()
            .Where(e => e.UserId == userId)
            .ToListAsync();

        if (startDate.HasValue)
            entries = entries.Where(e => e.EntryDate >= startDate.Value).ToList();

        if (endDate.HasValue)
            entries = entries.Where(e => e.EntryDate <= endDate.Value).ToList();

        foreach (var entry in entries)
        {
            var entryMoods = await _db.Table<EntryMood>()
                .Where(em => em.EntryId == entry.EntryId)
                .ToListAsync();

            foreach (var entryMood in entryMoods)
            {
                var mood = await _db.GetAsync<Mood>(entryMood.MoodId);
                if (mood != null && distribution.ContainsKey(mood.Category))
                {
                    distribution[mood.Category]++;
                }
            }
        }

        return distribution;
    }
}