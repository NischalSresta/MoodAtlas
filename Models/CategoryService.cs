using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MoodAtlas.Models;
using MoodAtlas.Config;
using SQLite;

namespace MoodAtlas.Services;

public class CategoryService
{
    private readonly SQLiteAsyncConnection _db;

    public CategoryService()
    {
        _db = DatabaseConfig.GetConnection();
    }

    public async Task<int> CreateCategoryAsync(Category category)
    {
        category.CreatedAt = DateTime.UtcNow;
        category.UpdatedAt = DateTime.UtcNow;
        return await _db.InsertAsync(category);
    }

    public async Task<Category> GetCategoryByIdAsync(int categoryId)
    {
        return await _db.GetAsync<Category>(categoryId);
    }

    public async Task<List<Category>> GetAllCategoriesAsync(int userId)
    {
        return await _db.Table<Category>()
            .Where(c => c.UserId == userId)
            .OrderBy(c => c.Name)
            .ToListAsync();
    }

    public async Task<int> UpdateCategoryAsync(Category category)
    {
        category.UpdatedAt = DateTime.UtcNow;
        return await _db.UpdateAsync(category);
    }

    public async Task<int> DeleteCategoryAsync(int categoryId)
    {
        return await _db.DeleteAsync<Category>(categoryId);
    }
}