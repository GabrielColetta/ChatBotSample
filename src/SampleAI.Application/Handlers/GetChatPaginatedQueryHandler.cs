using MediatR;
using SampleAI.Application.Contracts.Queries;
using SampleAI.Domain.Entities;
using SampleAI.Domain.Interfaces;
using SampleAI.Shared.Models;

namespace SampleAI.Application.Handlers;

public class GetChatPaginatedQueryHandler : IRequestHandler<GetChatPaginatedQuery, PaginatedResponse<Chat>>
{
    private readonly IChatRepository _chatRepository;

    public GetChatPaginatedQueryHandler(IChatRepository chatRepository)
    {
        _chatRepository = chatRepository;
    }

    public async Task<PaginatedResponse<Chat>> Handle(GetChatPaginatedQuery request, CancellationToken cancellationToken)
    {
        return await _chatRepository.GetPaginatedAsync(x => true, request.PaginateFilter, cancellationToken);
    }
}
