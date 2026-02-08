using ChessApp.API.Enums;
using ChessApp.API.DTOs.Openings;

namespace ChessApp.API.DTOs.OpeningNodes;

public sealed class CandidateMoveDto
{
    public int OpeningId { get; set; }
    public string OpeningName { get; set; } = null!;
    public string MoveSan { get; set; } = null!;
    public string MoveUci { get; set; } = null!;
    public bool IsFromCurrentOpening { get; set; }
}
