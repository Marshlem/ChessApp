using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace ChessApp.API.Data;

public sealed class OpeningNodeWriteRepository
{
    private readonly ApplicationDbContext _db;

    public OpeningNodeWriteRepository(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task DeleteSubtree(int userId, int openingId, int nodeId)
    {
        await _db.Database.ExecuteSqlRawAsync(
            "SELECT delete_opening_node_subtree(@u, @o, @n)",
            new NpgsqlParameter("u", userId),
            new NpgsqlParameter("o", openingId),
            new NpgsqlParameter("n", nodeId)
        );
    }
}
