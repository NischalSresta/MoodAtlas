using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SQLite;

namespace MoodAtlas.Models;

[Table("Categories")]
public class Category
{
    [PrimaryKey, AutoIncrement]
    public int CategoryId { get; set; }

    [Indexed]
    public int UserId { get; set; }

    public string Name { get; set; }

    public string Color { get; set; } = "#667eea";

    public string Icon { get; set; } = "📝";

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}
