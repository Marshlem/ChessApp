namespace ChessApp.Application.Training.GetTrainingSummary;

public sealed class GetTrainingSummaryResponse
{
    public int TotalPositions { get; set; }
    public int NewPositions { get; set; }
    public int DuePositions { get; set; }
}