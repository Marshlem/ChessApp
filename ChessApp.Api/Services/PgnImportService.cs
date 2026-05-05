using System.Text.RegularExpressions;
using ChessApp.API.Data;
using ChessApp.API.Enums;
using ChessApp.API.Infrastructure;
using ChessApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace ChessApp.API.Handlers.Repertoire;

public sealed class PgnImportService
{
    private const string CommentTokenPrefix = "__COMMENT_";

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
        var commentsByToken = new Dictionary<string, string>();
        var tokens = GetPgnTokens(pgnText, commentsByToken);

        var nodesByFen = await _db.OpeningNodes
            .Where(x => x.OpeningId == openingId)
            .ToDictionaryAsync(x => x.Fen, cancellationToken);

        nodesByFen[rootNode.Fen] = rootNode;

        var tokenIndex = 0;

        ImportLine(
            openingId,
            rootNode,
            rootNode.Fen,
            tokens,
            ref tokenIndex,
            nodesByFen,
            commentsByToken);
    }

    private void ImportLine(
        int openingId,
        OpeningNode startNode,
        string startFen,
        IReadOnlyList<string> tokens,
        ref int tokenIndex,
        Dictionary<string, OpeningNode> nodesByFen,
        IReadOnlyDictionary<string, string> commentsByToken)
    {
        var currentNode = startNode;
        var currentFen = startFen;

        OpeningNode? lastMoveNode = null;

        OpeningNode? variationStartNode = null;
        string? variationStartFen = null;

        while (tokenIndex < tokens.Count)
        {
            var token = tokens[tokenIndex];

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
                    ImportLine(
                        openingId,
                        variationStartNode,
                        variationStartFen,
                        tokens,
                        ref tokenIndex,
                        nodesByFen,
                        commentsByToken);
                }
                else
                {
                    SkipVariation(tokens, ref tokenIndex);
                }

                continue;
            }

            tokenIndex++;

            if (commentsByToken.TryGetValue(token, out var comment))
            {
                AddCommentToNode(lastMoveNode, comment);
                continue;
            }

            if (ShouldIgnoreToken(token))
                continue;

            var parentBeforeMove = currentNode;
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

            var nextNode = GetOrCreateNode(
                openingId,
                currentNode,
                newFen,
                token,
                moveUci,
                nodesByFen);

            currentNode = nextNode;
            currentFen = newFen;
            lastMoveNode = nextNode;

            variationStartNode = parentBeforeMove;
            variationStartFen = fenBeforeMove;
        }
    }

    private OpeningNode GetOrCreateNode(
        int openingId,
        OpeningNode parentNode,
        string fen,
        string moveSan,
        string moveUci,
        Dictionary<string, OpeningNode> nodesByFen)
    {
        if (nodesByFen.TryGetValue(fen, out var existingNode))
            return existingNode;

        var newNode = new OpeningNode
        {
            OpeningId = openingId,
            ParentNode = parentNode,
            Fen = fen,
            MoveSan = moveSan,
            MoveUci = moveUci,
            LineType = parentNode.LineType == default
                ? LineType.Main
                : parentNode.LineType,
            CreatedAtUtc = DateTime.UtcNow
        };

        _db.OpeningNodes.Add(newNode);
        nodesByFen[fen] = newNode;

        return newNode;
    }

    private static List<string> GetPgnTokens(
        string pgnText,
        Dictionary<string, string> commentsByToken)
    {
        var moveText = RemovePgnHeaders(pgnText);
        moveText = ReplaceBraceCommentsWithTokens(moveText, commentsByToken);
        moveText = RemoveLineComments(moveText);
        moveText = NormalizeMoveText(moveText);

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

    private static string ReplaceBraceCommentsWithTokens(
        string pgnText,
        Dictionary<string, string> commentsByToken)
    {
        var index = 0;

        return Regex.Replace(
            pgnText,
            @"\{([^}]*)\}",
            match =>
            {
                var token = $"{CommentTokenPrefix}{index++}__";
                commentsByToken[token] = NormalizeComment(match.Groups[1].Value);
                return $" {token} ";
            });
    }

    private static string NormalizeComment(string comment)
    {
        return comment
            .Replace("\r", " ")
            .Replace("\n", " ")
            .Trim();
    }

    private static string RemoveLineComments(string pgnText)
    {
        return Regex.Replace(
            pgnText,
            @";[^\r\n]*",
            " ");
    }

    private static string NormalizeMoveText(string moveText)
    {
        return moveText
            .Replace("(", " ( ")
            .Replace(")", " ) ");
    }

    private static void AddCommentToNode(OpeningNode? node, string comment)
    {
        if (node == null)
            return;

        if (string.IsNullOrWhiteSpace(comment))
            return;

        node.Comment = string.IsNullOrWhiteSpace(node.Comment)
            ? comment
            : $"{node.Comment}\n\n{comment}";
    }

    private static bool ShouldIgnoreToken(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
            return true;

        if (token is "1-0" or "0-1" or "1/2-1/2" or "*")
            return true;

        if (token.StartsWith("$"))
            return true;

        if (Regex.IsMatch(token, @"^\d+\.(\.\.)?$"))
            return true;

        return false;
    }

    private static void SkipVariation(
        IReadOnlyList<string> tokens,
        ref int tokenIndex)
    {
        var depth = 1;

        while (tokenIndex < tokens.Count && depth > 0)
        {
            if (tokens[tokenIndex] == "(")
                depth++;

            if (tokens[tokenIndex] == ")")
                depth--;

            tokenIndex++;
        }
    }
}