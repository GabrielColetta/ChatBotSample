using MediatR;
using SampleAI.Application.Contracts.Queries;
using SampleAI.Domain.Entities;
using SampleAI.Domain.Interfaces;
using SampleAI.Shared.Models;

namespace SampleAI.Application.Handlers;

public class GetConversationQueryHandler : IRequestHandler<GetConversationQuery, PaginatedResponse<Conversation>>
{
    private readonly IConversationRepository _conversationRepository;

    public GetConversationQueryHandler(IConversationRepository conversationRepository)
    {
        _conversationRepository = conversationRepository;
    }

    public async Task<PaginatedResponse<Conversation>> Handle(GetConversationQuery request, CancellationToken cancellationToken)
    {
        var response = await _conversationRepository.GetPaginatedAsync(
            x => x.ChatId == request.ChatId, request.Filter, cancellationToken);

        return response;
    }
}
