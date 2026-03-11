using MediatR;
using SampleAI.Application.Contracts.Queries;
using SampleAI.Application.Contracts.Responses;
using SampleAI.Domain.Entities;
using SampleAI.Domain.Interfaces;
using SampleAI.Shared.Models;
using System.Linq.Expressions;

namespace SampleAI.Application.Handlers;

public class GetConversationByIdQueryHandler : IRequestHandler<GetConversationByIdQuery, PaginatedResponse<GetConversationByIdResponse>>
{
    private readonly IConversationRepository _conversationRepository;

    public GetConversationByIdQueryHandler(IConversationRepository conversationRepository)
    {
        _conversationRepository = conversationRepository;
    }

    public async Task<PaginatedResponse<GetConversationByIdResponse>> Handle(GetConversationByIdQuery request, CancellationToken cancellationToken)
    {
        var response = await _conversationRepository.GetPaginatedAsync(
            GetExpression(request), GetSelector(), request.Filter, cancellationToken);

        return response;
    }

    private static Expression<Func<Conversation, bool>> GetExpression(GetConversationByIdQuery request)
    {
        return x => x.ChatId == request.ChatId;
    }

    private static Expression<Func<Conversation, GetConversationByIdResponse>> GetSelector()
    {
        return x => new GetConversationByIdResponse(x.ChatId, x.ChatRole.ToString(), x.Date, x.Content);
    }
}
