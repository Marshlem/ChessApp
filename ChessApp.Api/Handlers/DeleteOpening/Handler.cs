using ChessApp.API.Data;
using Microsoft.EntityFrameworkCore;

namespace ChessApp.API.Handlers.Openings;

public sealed class DeleteOpeningHandler
{
    private readonly ApplicationDbContext _db;

    public DeleteOpeningHandler(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task Execute(int userId, int openingId)
    {
        var opening = await _db.Openings
            .Where(o => o.Id == openingId && o.UserId == userId)
            .FirstOrDefaultAsync();

        if (opening == null)
            throw new InvalidOperationException("Opening not found.");

        var repItems = await _db.RepertoireItems
            .Where(x => x.OpeningId == openingId && x.UserId == userId)
            .ToListAsync();

        if (repItems.Count > 0)
            _db.RepertoireItems.RemoveRange(repItems);

        _db.Openings.Remove(opening);

        await _db.SaveChangesAsync();
    }
}
