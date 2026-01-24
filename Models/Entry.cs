namespace MoodAtlas.Models;

using SQLite;

[Table("Entries")]
public class Entry
{
    [PrimaryKey, AutoIncrement]
    public int EntryId { get; set; }

    [Indexed]
    public int CategoryId { get; set; }

    [Indexed]
    public int UserId { get; set; }

    public string Title { get; set; }

    public string Content { get; set; }

    [Indexed]
    public DateTime EntryDate { get; set; }

    public int WordCount { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    // Navigation properties (not stored in database)
    [Ignore]
    public List<Mood> Moods { get; set; } = new();

    [Ignore]
    public List<Tag> Tags { get; set; } = new();

    [Ignore]
    public Category Category { get; set; }

    // Helper properties
    [Ignore]
    public Mood PrimaryMood => Moods?.FirstOrDefault();

    [Ignore]
    public string MoodEmoji => PrimaryMood?.Emoji ?? "📝";
}