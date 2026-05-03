using ChessApp.API.Data;
using ChessApp.API.DTOs.OpeningNodes;
using Microsoft.EntityFrameworkCore;

namespace ChessApp.API.Handlers.OpeningNodes;

public sealed class UpdateNodeTypeHandler
{
    private readonly ApplicationDbContext _db;

    public UpdateNodeTypeHandler(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<UpdateNodeTypeResponse> Execute(
        int userId,
        int openingId,
        int nodeId,
        UpdateNodeTypeRequest request)
    {
        var nodeExists = await _db.OpeningNodes
            .AsNoTracking()
            .AnyAsync(x =>
                x.Id == nodeId &&
                x.OpeningId == openingId &&
                x.Opening.UserId == userId);

        if (!nodeExists)
            throw new KeyNotFoundException("Opening node not found");

        var allNodes = await _db.OpeningNodes
            .AsNoTracking()
            .Where(x => x.OpeningId == openingId)
            .Select(x => new NodeRelation(x.Id, x.ParentNodeId))
            .ToListAsync();

        var nodeIdsToUpdate = GetSubtreeNodeIds(allNodes, nodeId);

        await _db.OpeningNodes
            .Where(x => nodeIdsToUpdate.Contains(x.Id))
            .ExecuteUpdateAsync(setters =>
                setters.SetProperty(x => x.LineType, request.LineType));

        return new UpdateNodeTypeResponse
        {
            NodeId = nodeId,
            LineType = request.LineType
        };
    }

    private static List<int> GetSubtreeNodeIds(
        List<NodeRelation> nodes,
        int rootNodeId)
    {
        var result = new List<int>();
        var queue = new Queue<int>();

        result.Add(rootNodeId);
        queue.Enqueue(rootNodeId);

        while (queue.Count > 0)
        {
            var parentId = queue.Dequeue();

            var childIds = nodes
                .Where(x => x.ParentNodeId == parentId)
                .Select(x => x.Id);

            foreach (var childId in childIds)
            {
                result.Add(childId);
                queue.Enqueue(childId);
            }
        }

        return result;
    }

    private sealed record NodeRelation(int Id, int? ParentNodeId);
}