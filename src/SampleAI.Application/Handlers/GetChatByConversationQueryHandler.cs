using MediatR;
using SampleAI.Application.Contracts.Queries;
using SampleAI.Application.Services;
using SampleAI.Domain.Entities;
using SampleAI.Domain.Interfaces;
using SampleAI.Shared.Models;

namespace SampleAI.Application.Handlers;

public class GetChatByConversationQueryHandler : IRequestHandler<GetChatByConversationQuery, PaginatedResponse<Conversation>>
{
    private readonly IEmbeddingService _embeddingService;
    private readonly IConversationRepository _conversationRepository;

    public GetChatByConversationQueryHandler(IEmbeddingService embeddingService, IConversationRepository conversationRepository)
    {
        _embeddingService = embeddingService;
        _conversationRepository = conversationRepository;
    }

    public async Task<PaginatedResponse<Conversation>> Handle(GetChatByConversationQuery request, CancellationToken cancellationToken)
    {
        var embeddings = await _embeddingService.GetEmbeddingFromModelAsync(request.Search, cancellationToken);

        return await _conversationRepository.VectorSearchAsync(
            request.Filter,
            embeddings,
            cancellationToken);
    }
}
