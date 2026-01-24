using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MoodAtlas.Models;
using MoodAtlas.Config;
using SQLite;

namespace MoodAtlas.Services;

public class TagService
{
    private readonly SQLiteAsyncConnection _db;

    public TagService()
    {
        _db = DatabaseConfig.GetConnection();
    }

    public async Task<int> CreateTagAsync(Tag tag)
    {
        tag.CreatedAt = DateTime.UtcNow;
        tag.UpdatedAt = DateTime.UtcNow;
        return await _db.InsertAsync(tag);
    }

    public async Task<Tag> GetTagByIdAsync(int tagId)
    {
        return await _db.GetAsync<Tag>(tagId);
    }

    public async Task<List<Tag>> GetAllTagsAsync()
    {
        return await _db.Table<Tag>().OrderBy(t => t.Name).ToListAsync();
    }

    public async Task<List<Tag>> GetDefaultTagsAsync()
    {
        return await _db.Table<Tag>()
            .Where(t => t.IsDefault)
            .OrderBy(t => t.Name)
            .ToListAsync();
    }

    public async Task<List<Tag>> GetTagsForEntryAsync(int entryId)
    {
        var entryTags = await _db.Table<EntryTag>()
            .Where(et => et.EntryId == entryId)
            .ToListAsync();

        var tags = new List<Tag>();
        foreach (var entryTag in entryTags)
        {
            var tag = await _db.GetAsync<Tag>(entryTag.TagId);
            if (tag != null)
            {
                tags.Add(tag);
            }
        }
        return tags;
    }

    public async Task<int> AddTagToEntryAsync(int entryId, int tagId)
    {
        var entryTag = new EntryTag
        {
            EntryId = entryId,
            TagId = tagId,
            CreatedAt = DateTime.UtcNow
        };
        return await _db.InsertAsync(entryTag);
    }

    public async Task<int> RemoveTagFromEntryAsync(int entryId, int tagId)
    {
        return await _db.Table<EntryTag>()
            .Where(et => et.EntryId == entryId && et.TagId == tagId)
            .DeleteAsync();
    }

    public async Task<int> UpdateTagAsync(Tag tag)
    {
        tag.UpdatedAt = DateTime.UtcNow;
        return await _db.UpdateAsync(tag);
    }

    public async Task<int> DeleteTagAsync(int tagId)
    {
        // Delete from EntryTags first
        await _db.Table<EntryTag>().DeleteAsync(et => et.TagId == tagId);
        return await _db.DeleteAsync<Tag>(tagId);
    }

    public async Task<Dictionary<string, int>> GetTagFrequencyAsync(int userId, DateTime? startDate = null, DateTime? endDate = null)
    {
        var frequency = new Dictionary<string, int>();

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
            var entryTags = await _db.Table<EntryTag>()
                .Where(et => et.EntryId == entry.EntryId)
                .ToListAsync();

            foreach (var entryTag in entryTags)
            {
                var tag = await _db.GetAsync<Tag>(entryTag.TagId);
                if (tag != null)
                {
                    if (frequency.ContainsKey(tag.Name))
                        frequency[tag.Name]++;
                    else
                        frequency[tag.Name] = 1;
                }
            }
        }

        return frequency.OrderByDescending(f => f.Value).ToDictionary(f => f.Key, f => f.Value);
    }
}