using SQLite;

namespace MoodAtlas.Config;

public class DatabaseConfig
{
    private static SQLiteAsyncConnection _connection;
    private const string DatabaseFileName = "moodatlas.db";

    public static SQLiteAsyncConnection GetConnection()
    {
        Console.WriteLine("Getting database connection...");
        if (_connection == null)
        {
            var dbPath = Path.Combine(FileSystem.AppDataDirectory, DatabaseFileName);
            System.Diagnostics.Debug.WriteLine($"📁 DATABASE PATH: {dbPath}");
            _connection = new SQLiteAsyncConnection(dbPath);
            InitializeDatabase();
        }

        return _connection;
    }

    private static async void InitializeDatabase()
    {
        await _connection.CreateTableAsync<Models.User>();
        await _connection.CreateTableAsync<Models.Entry>();
        await _connection.CreateTableAsync<Models.Category>();
        await _connection.CreateTableAsync<Models.Mood>();
        await _connection.CreateTableAsync<Models.Tag>();
        await _connection.CreateTableAsync<Models.EntryMood>();
        await _connection.CreateTableAsync<Models.EntryTag>();

        await SeedDefaultData();
    }

    private static async Task SeedDefaultData()
    {
        // Check if moods already exist
        var moodCount = await _connection.Table<Models.Mood>().CountAsync();
        if (moodCount == 0)
        {
            var defaultMoods = new List<Models.Mood>
            {
                // Positive moods
                new() { Name = "Happy", Category = "Positive", Emoji = "😊", Description = "Feeling joyful and content", IsDefault = true },
                new() { Name = "Excited", Category = "Positive", Emoji = "🎉", Description = "Full of enthusiasm", IsDefault = true },
                new() { Name = "Relaxed", Category = "Positive", Emoji = "😌", Description = "Calm and at ease", IsDefault = true },
                new() { Name = "Grateful", Category = "Positive", Emoji = "🙏", Description = "Thankful and appreciative", IsDefault = true },
                new() { Name = "Confident", Category = "Positive", Emoji = "💪", Description = "Self-assured and strong", IsDefault = true },
                
                // Neutral moods
                new() { Name = "Calm", Category = "Neutral", Emoji = "😐", Description = "Peaceful and balanced", IsDefault = true },
                new() { Name = "Thoughtful", Category = "Neutral", Emoji = "🤔", Description = "Deep in thought", IsDefault = true },
                new() { Name = "Curious", Category = "Neutral", Emoji = "🧐", Description = "Interested and inquisitive", IsDefault = true },
                new() { Name = "Nostalgic", Category = "Neutral", Emoji = "📸", Description = "Reminiscing about the past", IsDefault = true },
                new() { Name = "Bored", Category = "Neutral", Emoji = "😒", Description = "Lacking interest or excitement", IsDefault = true },
                
                // Negative moods
                new() { Name = "Sad", Category = "Negative", Emoji = "😢", Description = "Feeling unhappy or sorrowful", IsDefault = true },
                new() { Name = "Angry", Category = "Negative", Emoji = "😠", Description = "Feeling strong annoyance", IsDefault = true },
                new() { Name = "Stressed", Category = "Negative", Emoji = "😫", Description = "Under mental or emotional pressure", IsDefault = true },
                new() { Name = "Lonely", Category = "Negative", Emoji = "😔", Description = "Feeling isolated or alone", IsDefault = true },
                new() { Name = "Anxious", Category = "Negative", Emoji = "😰", Description = "Worried or uneasy", IsDefault = true }
            };

            foreach (var mood in defaultMoods)
            {
                mood.CreatedAt = DateTime.UtcNow;
                mood.UpdatedAt = DateTime.UtcNow;
                await _connection.InsertAsync(mood);
            }
        }

        // Check if default tags exist
        var tagCount = await _connection.Table<Models.Tag>().CountAsync();
        if (tagCount == 0)
        {
            var defaultTags = new[]
            {
                "Work", "Career", "Studies", "Family", "Friends", "Relationships",
                "Health", "Fitness", "Personal Growth", "Self-care", "Hobbies", "Travel",
                "Nature", "Finance", "Spirituality", "Birthday", "Holiday", "Vacation",
                "Celebration", "Exercise", "Reading", "Writing", "Cooking", "Meditation",
                "Yoga", "Music", "Shopping", "Parenting", "Projects", "Planning", "Reflection"
            };

            foreach (var tagName in defaultTags)
            {
                var tag = new Models.Tag
                {
                    Name = tagName,
                    IsDefault = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                await _connection.InsertAsync(tag);
            }
        }
    }

    public static string GetDatabasePath()
    {
        return Path.Combine(FileSystem.AppDataDirectory, DatabaseFileName);
    }
}