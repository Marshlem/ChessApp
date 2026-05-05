using System.Text.RegularExpressions;
using ChessApp.API.Data;
using ChessApp.API.Enums;
using ChessApp.API.Infrastructure;
using ChessApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace ChessApp.API.Handlers.Repertoire;

public sealed class PgnImportService
{
    private readonly ApplicationDbContext _db;

    public PgnImportService(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task Import(
        int openingId,
        OpeningNode rootNode,
        string pgnText,
        CancellationToken cancellationToken)
    {
        var moveTokens = GetPgnMoveTokens(pgnText);

        var nodesByFen = await _db.OpeningNodes
            .Where(x => x.OpeningId == openingId)
            .ToDictionaryAsync(x => x.Fen, cancellationToken);

        nodesByFen[rootNode.Fen] = rootNode;

        var tokenIndex = 0;

        ImportMoveLine(
            openingId,
            rootNode,
            rootNode.Fen,
            moveTokens,
            ref tokenIndex,
            nodesByFen);
    }

    private void ImportMoveLine(
        int openingId,
        OpeningNode startNode,
        string startFen,
        IReadOnlyList<string> moveTokens,
        ref int tokenIndex,
        Dictionary<string, OpeningNode> nodesByFen)
    {
        var currentNode = startNode;
        var currentFen = startFen;

        OpeningNode? variationStartNode = null;
        string? variationStartFen = null;

        while (tokenIndex < moveTokens.Count)
        {
            var token = moveTokens[tokenIndex];

            if (token == ")")
            {
                tokenIndex++;
                return;
            }

            if (token == "(")
            {
                tokenIndex++;

                if (variationStartNode != null && variationStartFen != null)
                {
                    ImportMoveLine(
                        openingId,
                        variationStartNode,
                        variationStartFen,
                        moveTokens,
                        ref tokenIndex,
                        nodesByFen);
                }
                else
                {
                    SkipVariationBlock(moveTokens, ref tokenIndex);
                }

                continue;
            }

            tokenIndex++;

            if (ShouldIgnoreMoveToken(token))
                continue;

            var parentNodeBeforeMove = currentNode;
            var fenBeforeMove = currentFen;

            if (!ChessRules.TryApplySan(
                    currentFen,
                    token,
                    out var newFen,
                    out var moveUci))
            {
                throw new InvalidOperationException(
                    $"Invalid PGN move '{token}' from FEN '{currentFen}'.");
            }

            if (!nodesByFen.TryGetValue(newFen, out var nextNode))
            {
                nextNode = new OpeningNode
                {
                    OpeningId = openingId,
                    ParentNode = currentNode,
                    Fen = newFen,
                    MoveSan = token,
                    MoveUci = moveUci,
                    LineType = currentNode.LineType == default
                        ? LineType.Main
                        : currentNode.LineType,
                    CreatedAtUtc = DateTime.UtcNow
                };

                _db.OpeningNodes.Add(nextNode);
                nodesByFen[newFen] = nextNode;
            }

            currentNode = nextNode;
            currentFen = newFen;

            // PGN variantas po ėjimo yra alternatyva iš pozicijos prieš tą ėjimą.
            // Pvz.: 1. d4 d5 (1... Nf6)
            // Nf6 turi kabėti nuo pozicijos po 1. d4, ne po 1... d5.
            variationStartNode = parentNodeBeforeMove;
            variationStartFen = fenBeforeMove;
        }
    }

    private static List<string> GetPgnMoveTokens(string pgnText)
    {
        var moveText = RemovePgnHeaders(pgnText);
        moveText = RemovePgnBraceComments(moveText);
        moveText = RemovePgnLineComments(moveText);
        moveText = NormalizePgnMoveText(moveText);

        return moveText
            .Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(x => x.Trim())
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .ToList();
    }

    private static string RemovePgnHeaders(string pgnText)
    {
        return Regex.Replace(
            pgnText,
            @"^\s*\[.*?\]\s*$",
            "",
            RegexOptions.Multiline);
    }

    private static string RemovePgnBraceComments(string pgnText)
    {
        return Regex.Replace(
            pgnText,
            @"\{[^}]*\}",
            " ");
    }

    private static string RemovePgnLineComments(string pgnText)
    {
        return Regex.Replace(
            pgnText,
            @";[^\r\n]*",
            " ");
    }

    private static string NormalizePgnMoveText(string moveText)
    {
        return moveText
            .Replace("(", " ( ")
            .Replace(")", " ) ");
    }

    private static bool ShouldIgnoreMoveToken(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
            return true;

        if (token is "1-0" or "0-1" or "1/2-1/2" or "*")
            return true;

        if (token.StartsWith("$"))
            return true;

        // Pvz.: 1. 2. 15. 1... 23...
        if (Regex.IsMatch(token, @"^\d+\.(\.\.)?$"))
            return true;

        return false;
    }

    private static void SkipVariationBlock(
        IReadOnlyList<string> moveTokens,
        ref int tokenIndex)
    {
        var depth = 1;

        while (tokenIndex < moveTokens.Count && depth > 0)
        {
            if (moveTokens[tokenIndex] == "(")
                depth++;

            if (moveTokens[tokenIndex] == ")")
                depth--;

            tokenIndex++;
        }
    }
}