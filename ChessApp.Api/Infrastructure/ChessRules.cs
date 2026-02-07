using System.Text.RegularExpressions;
using ChessDotNet;

namespace ChessApp.API.Infrastructure;

public static class ChessRules
{
    private static readonly Regex UciRegex =
        new(@"^[a-h][1-8][a-h][1-8][qrbn]?$", RegexOptions.Compiled);

    public static bool TryApplyUci(
        string fen,
        string moveUci,
        out string newFen,
        out string moveSan)
    {
        newFen = null!;
        moveSan = null!;

        if (string.IsNullOrWhiteSpace(moveUci))
            return false;

        moveUci = moveUci.ToLowerInvariant();

        if (!UciRegex.IsMatch(moveUci))
            return false;

        var game = new ChessGame(fen);

        var from = new Position(moveUci[..2]);
        var to   = new Position(moveUci.Substring(2, 2));

        char? promotion = moveUci.Length == 5 ? moveUci[4] : null;

        var move = new Move(from, to, game.WhoseTurn, promotion);

        var result = game.MakeMove(move, true);
        if ((result & MoveType.Invalid) == MoveType.Invalid)
            return false;

        newFen = game.GetFen();
        moveSan = move.ToString(); 

        return true;
    }
}
