using Microsoft.EntityFrameworkCore;

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
        var openingExists = await _db.Openings
            .AnyAsync(x => x.Id == openingId && x.UserId == userId);

        if (!openingExists)
            return;

        var nodes = await _db.OpeningNodes
            .Where(x => x.OpeningId == openingId)
            .ToListAsync();

        var nodeById = nodes.ToDictionary(x => x.Id);
        if (!nodeById.ContainsKey(nodeId))
            return;

        var childrenByParentId = nodes
            .Where(x => x.ParentNodeId.HasValue)
            .GroupBy(x => x.ParentNodeId!.Value)
            .ToDictionary(g => g.Key, g => g.ToList());

        var idsToDelete = new HashSet<int>();
        var stack = new Stack<int>();
        stack.Push(nodeId);

        while (stack.Count > 0)
        {
            var currentId = stack.Pop();

            if (!idsToDelete.Add(currentId))
                continue;

            if (childrenByParentId.TryGetValue(currentId, out var children))
            {
                foreach (var child in children)
                    stack.Push(child.Id);
            }
        }

        var nodesToDelete = nodes
            .Where(x => idsToDelete.Contains(x.Id))
            .ToList();

        _db.OpeningNodes.RemoveRange(nodesToDelete);
        await _db.SaveChangesAsync();
    }
}