using ChessApp.Application.Training.GetNextTrainingPosition;

namespace ChessApp.Application.Training.SubmitTrainingAnswer;

public sealed class SubmitTrainingAnswerResponse
{
    public bool IsCorrect { get; set; }
    public int CorrectOpeningNodeId { get; set; }
    public string CorrectMoveSan { get; set; } = null!;
    public string? CorrectMoveUci { get; set; }

    public int Bucket { get; set; }
    public int TrainedCount { get; set; }
    public int FailedCount { get; set; }
    public DateTime? NextDueAtUtc { get; set; }

    public int CurrentOpeningNodeId { get; set; }
    public string CurrentFen { get; set; } = null!;
    public string CurrentSideToMove { get; set; } = null!;
    public List<TrainingMoveOptionDto> MoveOptions { get; set; } = new();
}