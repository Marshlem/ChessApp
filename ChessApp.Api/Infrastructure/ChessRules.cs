using System.Text.RegularExpressions;
using ChessDotNet;

namespace ChessApp.API.Infrastructure;

public static class ChessRules
{
    private static readonly Regex UciRegex =
        new(@"^[a-h][1-8][a-h][1-8][qrbn]?$", RegexOptions.Compiled);

    private static readonly Regex SanRegex =
        new(@"^(?<piece>[KQRBN])?(?<fromFile>[a-h])?(?<fromRank>[1-8])?x?(?<to>[a-h][1-8])(?:=(?<promotion>[QRBN]))?[+#]?$",
            RegexOptions.Compiled);

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

        try
        {
            var game = new ChessGame(fen);

            var from = new Position(moveUci[..2]);
            var to = new Position(moveUci.Substring(2, 2));

            char? promotion = moveUci.Length == 5 ? moveUci[4] : null;

            var move = new Move(from, to, game.WhoseTurn, promotion);

            var result = game.MakeMove(move, false);
            if ((result & MoveType.Invalid) == MoveType.Invalid)
                return false;

            newFen = game.GetFen();
            moveSan = move.ToString();

            return true;
        }
        catch
        {
            return false;
        }
    }

public static bool TryApplySan(
    string fen,
    string moveSan,
    out string newFen,
    out string moveUci)
{
    newFen = null!;
    moveUci = null!;

    if (string.IsNullOrWhiteSpace(moveSan))
        return false;

    var san = NormalizeSan(moveSan);

    if (TryApplyCastlingSan(fen, san, out newFen, out moveUci))
        return true;

    foreach (var candidateUci in GetAllCandidateUciMoves())
    {
        if (!MoveMatchesSan(fen, candidateUci, san))
            continue;

        if (!TryApplyUci(fen, candidateUci, out var candidateFen, out _))
            continue;

        newFen = candidateFen;
        moveUci = candidateUci;
        return true;
    }

    return false;
}

private static IEnumerable<string> GetAllCandidateUciMoves()
{
    for (var fromFile = 'a'; fromFile <= 'h'; fromFile++)
    {
        for (var fromRank = '1'; fromRank <= '8'; fromRank++)
        {
            for (var toFile = 'a'; toFile <= 'h'; toFile++)
            {
                for (var toRank = '1'; toRank <= '8'; toRank++)
                {
                    var from = $"{fromFile}{fromRank}";
                    var to = $"{toFile}{toRank}";

                    if (from == to)
                        continue;

                    yield return from + to;

                    if (toRank is '1' or '8')
                    {
                        yield return from + to + "q";
                        yield return from + to + "r";
                        yield return from + to + "b";
                        yield return from + to + "n";
                    }
                }
            }
        }
    }
}

private static bool MoveMatchesSan(string fen, string moveUci, string san)
{
    var match = SanRegex.Match(san);

    if (!match.Success)
        return false;

    var game = new ChessGame(fen);

    var from = moveUci[..2];
    var to = moveUci.Substring(2, 2);

    var piece = game.GetPieceAt(new Position(from));

    if (piece == null)
        return false;

    var pieceLetter = match.Groups["piece"].Value;
    var expectedPiece = string.IsNullOrEmpty(pieceLetter)
        ? 'P'
        : pieceLetter[0];

    if (!PieceMatches(piece, expectedPiece))
        return false;

    var expectedTo = match.Groups["to"].Value;

    if (to != expectedTo)
        return false;

    var fromFile = match.Groups["fromFile"].Value;
    if (!string.IsNullOrEmpty(fromFile) && from[0] != fromFile[0])
        return false;

    var fromRank = match.Groups["fromRank"].Value;
    if (!string.IsNullOrEmpty(fromRank) && from[1] != fromRank[0])
        return false;

    var promotion = match.Groups["promotion"].Value;
    if (!string.IsNullOrEmpty(promotion))
    {
        if (moveUci.Length != 5)
            return false;

        if (char.ToUpperInvariant(moveUci[4]) != promotion[0])
            return false;
    }

    return true;
}

private static string NormalizeSan(string san)
{
    return san
        .Trim()
        .Replace("0-0-0", "O-O-O")
        .Replace("0-0", "O-O")
        .Replace("++", "+");
}

private static bool TryApplyCastlingSan(
    string fen,
    string san,
    out string newFen,
    out string moveUci)
{
    newFen = null!;
    moveUci = null!;

    if (san is not ("O-O" or "O-O-O"))
        return false;

    var game = new ChessGame(fen);
    var isWhite = game.WhoseTurn == Player.White;

    moveUci = san == "O-O"
        ? isWhite ? "e1g1" : "e8g8"
        : isWhite ? "e1c1" : "e8c8";

    return TryApplyUci(fen, moveUci, out newFen, out _);
}

    private static IEnumerable<Move> GetLegalCandidateMoves(ChessGame game)
    {
        for (var file = 'a'; file <= 'h'; file++)
        {
            for (var rank = '1'; rank <= '8'; rank++)
            {
                var from = new Position($"{file}{rank}");

                foreach (var move in game.GetValidMoves(from))
                {
                    yield return move;
                }
            }
        }
    }

    private static string ToUci(Move move)
    {
        var uci = $"{move.OriginalPosition}{move.NewPosition}".ToLowerInvariant();

        if (move.Promotion.HasValue)
            uci += char.ToLowerInvariant(move.Promotion.Value);

        return uci;
    }

    private static bool PieceMatches(Piece piece, char expectedPiece)
    {
        var fenChar = char.ToUpperInvariant(piece.GetFenCharacter());

        return expectedPiece switch
        {
            'P' => fenChar == 'P',
            'N' => fenChar == 'N',
            'B' => fenChar == 'B',
            'R' => fenChar == 'R',
            'Q' => fenChar == 'Q',
            'K' => fenChar == 'K',
            _ => false
        };
    }

    private static string NormalizeCastling(string san)
    {
        return san
            .Replace("0-0-0", "O-O-O")
            .Replace("0-0", "O-O");
    }
}