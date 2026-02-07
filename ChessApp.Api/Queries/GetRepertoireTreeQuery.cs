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
        .Select(x => new RepertoireItemDto
        {
            Id = x.Id,
            OpeningId = x.Opening != null ? x.Opening.Id : null,
            Name = x.Name,
            Color = x.Color
        })
        .ToListAsync();
}
}
