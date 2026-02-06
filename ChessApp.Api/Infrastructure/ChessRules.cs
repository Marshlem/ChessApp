using ChessDotNet;

namespace ChessApp.API.Infrastructure;

public static class ChessRules
{
    public static (bool ok, string? newFen, string? moveSan) TryApplyUci(
        string fen,
        string moveUci)
    {
        if (string.IsNullOrWhiteSpace(moveUci))
            return (false, null, null);

        if (moveUci.Length != 4 && moveUci.Length != 5)
            return (false, null, null);

        var game = new ChessGame(fen);

        var from = new Position(moveUci.Substring(0, 2));
        var to = new Position(moveUci.Substring(2, 2));

        // 1️⃣ ar yra figūra
        var piece = game.GetPieceAt(from);
        if (piece == null)
            return (false, null, null);

        // 2️⃣ ar figūra priklauso tam, kieno eilė
        if (piece.Owner != game.WhoseTurn)
            return (false, null, null);

        char? promotion = null;

        // 3️⃣ promotion logika
        if (piece is ChessDotNet.Pieces.Pawn)
        {
            var lastRank = piece.Owner == Player.White ? 8 : 1;

            if (to.Rank == lastRank)
            {
                if (moveUci.Length != 5)
                    return (false, null, null); // promotion privalomas

                promotion = moveUci[4] switch
                {
                    'q' or 'Q' => 'q',
                    'r' or 'R' => 'r',
                    'b' or 'B' => 'b',
                    'n' or 'N' => 'n',
                    _ => null
                };

                if (promotion == null)
                    return (false, null, null);
            }
        }

        var move = new Move(from, to, game.WhoseTurn, promotion);

        var result = game.MakeMove(move, true);

        if ((result & MoveType.Invalid) == MoveType.Invalid)
            return (false, null, null);

        var san = move.ToString();

        return (true, game.GetFen(), san);
    }
}
