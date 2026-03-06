using MediatR;
using ChessApp.Application.Training.SubmitTrainingAnswer;

namespace ChessApp.Application.Training.SubmitTrainingAnswer;

public sealed record SubmitTrainingAnswerCommand(
    int UserId,
    int OpeningNodeId,
    int SelectedOpeningNodeId) : IRequest<SubmitTrainingAnswerResponse>;