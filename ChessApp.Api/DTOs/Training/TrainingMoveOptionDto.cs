namespace ChessApp.Application.Training.GetNextTrainingPosition;
public sealed class TrainingMoveOptionDto
{
    public int OpeningNodeId { get; set; }
    public string MoveSan { get; set; } = null!;
    public string? MoveUci { get; set; }
}