using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SQLite;

namespace MoodAtlas.Models;

[Table("Moods")]
public class Mood
{
    [PrimaryKey, AutoIncrement]
    public int MoodId { get; set; }

    public string Name { get; set; }

    [Indexed]
    public string Category { get; set; } // Positive, Neutral, Negative

    public string Emoji { get; set; }

    public string Description { get; set; }

    public bool IsDefault { get; set; } = true;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}

// Junction table for Entry-Mood relationship
[Table("EntryMoods")]
public class EntryMood
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [Indexed]
    public int EntryId { get; set; }

    [Indexed]
    public int MoodId { get; set; }

    public bool IsPrimary { get; set; }

    public DateTime CreatedAt { get; set; }
}