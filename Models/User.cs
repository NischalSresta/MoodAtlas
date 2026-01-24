namespace MoodAtlas.Models;

using SQLite;

public class User
{
    [PrimaryKey, AutoIncrement]
    public int UserId { get; set; }

    public string Username { get; set; }

    public string SecurityPIN { get; set; }
    
    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
    
    public bool IsDarkMode { get; set; }
}
