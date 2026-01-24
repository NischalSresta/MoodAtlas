using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SQLite;

namespace MoodAtlas.Models;

[Table("Tags")]
public class Tag
{
    [PrimaryKey, AutoIncrement]
    public int TagId { get; set; }

    [Indexed]
    public int UserId { get; set; }

    public string Name { get; set; }

    public string Color { get; set; } = "#6c757d";

    public bool IsDefault { get; set; } = true;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}

[Table("EntryTags")]
public class EntryTag
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [Indexed]
    public int EntryId { get; set; }

    [Indexed]
    public int TagId { get; set; }

    public DateTime CreatedAt { get; set; }
}