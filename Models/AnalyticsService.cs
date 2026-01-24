using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MoodAtlas.Models;
using MoodAtlas.Config;
using SQLite;

namespace MoodAtlas.Services;

public class AnalyticsService
{
    private readonly SQLiteAsyncConnection _db;
    private readonly EntryService _entryService;
    private readonly MoodService _moodService;
    private readonly TagService _tagService;

    public AnalyticsService()
    {
        _db = DatabaseConfig.GetConnection();
        _entryService = new EntryService();
        _moodService = new MoodService();
        _tagService = new TagService();
    }

    public class AnalyticsData
    {
        public int TotalEntries { get; set; }
        public int CurrentStreak { get; set; }
        public int LongestStreak { get; set; }
        public int MissedDays { get; set; }
        public Dictionary<string, int> MoodDistribution { get; set; }
        public Dictionary<string, int> TagFrequency { get; set; }
        public string MostFrequentMood { get; set; }
        public List<string> MostUsedTags { get; set; }
        public int AverageWordCount { get; set; }
        public Dictionary<DateTime, int> WordCountTrend { get; set; }
    }

    public async Task<AnalyticsData> GetAnalyticsAsync(int userId, DateTime? startDate = null, DateTime? endDate = null)
    {
        var entries = await _entryService.GetEntriesByUserIdAsync(userId);

        // Filter by date range if provided
        if (startDate.HasValue)
            entries = entries.Where(e => e.EntryDate >= startDate.Value).ToList();

        if (endDate.HasValue)
            entries = entries.Where(e => e.EntryDate <= endDate.Value).ToList();

        return new AnalyticsData
        {
            TotalEntries = entries.Count,
            CurrentStreak = await CalculateCurrentStreakAsync(userId),
            LongestStreak = await CalculateLongestStreakAsync(userId),
            MissedDays = await CalculateMissedDaysAsync(userId, startDate, endDate),
            MoodDistribution = await _moodService.GetMoodDistributionAsync(userId, startDate, endDate),
            TagFrequency = await _tagService.GetTagFrequencyAsync(userId, startDate, endDate),
            MostFrequentMood = await GetMostFrequentMoodAsync(userId, startDate, endDate),
            MostUsedTags = await GetMostUsedTagsAsync(userId, startDate, endDate),
            AverageWordCount = CalculateAverageWordCount(entries),
            WordCountTrend = CalculateWordCountTrend(entries)
        };
    }

    private async Task<int> CalculateCurrentStreakAsync(int userId)
    {
        var entries = await _entryService.GetEntriesByUserIdAsync(userId);
        if (entries.Count == 0) return 0;

        var sortedEntries = entries.OrderByDescending(e => e.EntryDate).ToList();
        int streak = 0;
        var currentDate = DateTime.Now.Date;

        // Check if today has an entry
        if (sortedEntries.Any(e => e.EntryDate.Date == currentDate))
        {
            streak++;
            currentDate = currentDate.AddDays(-1);
        }

        // Check consecutive days backward
        while (sortedEntries.Any(e => e.EntryDate.Date == currentDate))
        {
            streak++;
            currentDate = currentDate.AddDays(-1);
        }

        return streak;
    }

    private async Task<int> CalculateLongestStreakAsync(int userId)
    {
        var entries = await _entryService.GetEntriesByUserIdAsync(userId);
        if (entries.Count == 0) return 0;

        var sortedEntries = entries.OrderBy(e => e.EntryDate).Select(e => e.EntryDate.Date).Distinct().ToList();
        int maxStreak = 1;
        int currentStreak = 1;

        for (int i = 1; i < sortedEntries.Count; i++)
        {
            if ((sortedEntries[i] - sortedEntries[i - 1]).Days == 1)
            {
                currentStreak++;
                maxStreak = Math.Max(maxStreak, currentStreak);
            }
            else
            {
                currentStreak = 1;
            }
        }

        return maxStreak;
    }

    private async Task<int> CalculateMissedDaysAsync(int userId, DateTime? startDate, DateTime? endDate)
    {
        var entries = await _entryService.GetEntriesByUserIdAsync(userId);
        var entryDates = entries.Select(e => e.EntryDate.Date).Distinct().ToList();

        if (!startDate.HasValue) startDate = entryDates.Min();
        if (!endDate.HasValue) endDate = DateTime.Now.Date;

        int totalDays = (int)(endDate.Value.Date - startDate.Value.Date).TotalDays + 1;
        int daysWithEntries = entryDates.Count(e => e >= startDate.Value.Date && e <= endDate.Value.Date);

        return totalDays - daysWithEntries;
    }

    private async Task<string> GetMostFrequentMoodAsync(int userId, DateTime? startDate, DateTime? endDate)
    {
        var moodDistribution = await _moodService.GetMoodDistributionAsync(userId, startDate, endDate);

        if (moodDistribution.Sum(m => m.Value) == 0)
            return "No moods recorded";

        var maxCategory = moodDistribution.OrderByDescending(m => m.Value).First();
        return $"{maxCategory.Key} ({maxCategory.Value} entries)";
    }

    private async Task<List<string>> GetMostUsedTagsAsync(int userId, DateTime? startDate, DateTime? endDate)
    {
        var tagFrequency = await _tagService.GetTagFrequencyAsync(userId, startDate, endDate);
        return tagFrequency.Take(5).Select(t => $"{t.Key} ({t.Value})").ToList();
    }

    private int CalculateAverageWordCount(List<Models.Entry> entries)
    {
        if (entries.Count == 0) return 0;
        return (int)entries.Average(e => e.WordCount);
    }

    private Dictionary<DateTime, int> CalculateWordCountTrend(List<Models.Entry> entries)
    {
        var trend = new Dictionary<DateTime, int>();

        if (entries.Count == 0) return trend;

        // Group by week
        var weeklyGroups = entries
            .GroupBy(e => GetWeekStart(e.EntryDate))
            .OrderBy(g => g.Key);

        foreach (var group in weeklyGroups)
        {
            trend[group.Key] = (int)group.Average(e => e.WordCount);
        }

        return trend;
    }

    private DateTime GetWeekStart(DateTime date)
    {
        int diff = (7 + (date.DayOfWeek - DayOfWeek.Monday)) % 7;
        return date.AddDays(-1 * diff).Date;
    }
}