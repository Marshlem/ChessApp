using ChessApp.API.Data;
using ChessApp.API.DTOs.OpeningNodes;

public sealed class GetCandidateMovesQuery
{
    private readonly OpeningNodeReadRepository _repo;

    public GetCandidateMovesQuery(OpeningNodeReadRepository repo)
    {
        _repo = repo;
    }
        
    public Task<List<CandidateMoveDto>> Execute(
        int userId,
        int openingId,
        int parentNodeId)
    {
        return _repo.GetCandidateMoves(userId, openingId, parentNodeId);
    }
}
