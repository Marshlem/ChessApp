using ChessApp.API.Enums;

namespace ChessApp.API.DTOs.OpeningNodes;

public sealed class UpdateOpeningNodeMoveEvaluationRequest
{
    public MoveEvaluation? Evaluation { get; set; }
}