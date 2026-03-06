namespace ChessApp.Application.Training.GetNextTrainingPosition;

public sealed class GetNextTrainingPositionResponse
{
    public int OpeningNodeId { get; set; }
    public int OpeningId { get; set; }
    public string OpeningName { get; set; } = null!;
    public string Fen { get; set; } = null!;
    public string SideToMove { get; set; } = null!;
    public int RepertoireColor { get; set; }
    public List<TrainingMoveOptionDto> MoveOptions { get; set; } = new();
}