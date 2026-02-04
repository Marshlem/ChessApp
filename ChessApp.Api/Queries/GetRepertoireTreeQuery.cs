using ChessApp.API.Data;
using ChessApp.API.DTOs.Repertoire;
using Microsoft.EntityFrameworkCore;

public sealed class GetRepertoireTreeQuery
{
    private readonly ApplicationDbContext _db;

    public GetRepertoireTreeQuery(ApplicationDbContext db)
    {
        _db = db;
    }

public async Task<List<RepertoireItemDto>> Execute(int userId)
{
    return await _db.RepertoireItems
        .Where(x => x.UserId == userId)
        .OrderBy(x => x.Color)       
        .ThenBy(x => x.SortOrder)
        .Select(x => new RepertoireItemDto
        {
            Id = x.Id,
            Name = x.Name,
            Type = x.Type,
            Color = x.Color,
            SortOrder = x.SortOrder
        })
        .ToListAsync();
}
}
