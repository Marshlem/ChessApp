using ChessApp.API.Data;

namespace ChessApp.API.Handlers.OpeningNodes;

public sealed class DeleteOpeningNodeSubtreeHandler
{
    private readonly OpeningNodeWriteRepository _repo;

    public DeleteOpeningNodeSubtreeHandler(OpeningNodeWriteRepository repo)
    {
        _repo = repo;
    }

    public async Task Execute(int userId, int openingId, int nodeId)
    {
        await _repo.DeleteSubtree(userId, openingId, nodeId);
    }
}
