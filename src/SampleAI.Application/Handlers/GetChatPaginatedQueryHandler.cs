using MediatR;
using SampleAI.Application.Contracts.Queries;
using SampleAI.Application.Contracts.Responses;
using SampleAI.Domain.Entities;
using SampleAI.Domain.Interfaces;
using SampleAI.Shared.Models;
using System.Linq.Expressions;

namespace SampleAI.Application.Handlers;

public class GetChatPaginatedQueryHandler : IRequestHandler<GetChatPaginatedQuery, PaginatedResponse<GetChatResponse>>
{
    private readonly IChatRepository _chatRepository;

    public GetChatPaginatedQueryHandler(IChatRepository chatRepository)
    {
        _chatRepository = chatRepository;
    }

    public async Task<PaginatedResponse<GetChatResponse>> Handle(GetChatPaginatedQuery request, CancellationToken cancellationToken)
    {
        return await _chatRepository.GetPaginatedAsync(x => true, GetSelector(), request.PaginateFilter, cancellationToken);
    }

    private static Expression<Func<Chat, GetChatResponse>> GetSelector()
    {
        return x => new GetChatResponse(x.Id, x.Title, x.Date);
    }
}
