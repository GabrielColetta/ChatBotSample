using MediatR;
using Microsoft.AspNetCore.Mvc;
using SampleAI.Api.Models.Responses;
using SampleAI.Application.Contracts.Queries;
using SampleAI.Shared.Filters;

namespace SampleAI.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ChatController : ControllerBase
{
    private const int CurrentPage = 0;
    private const int PerPage = 10;

    private readonly IMediator _mediator;
    private readonly ILogger<ChatController> _logger;

    public ChatController(IMediator mediator, ILogger<ChatController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Get the chat conversation paginated.
    /// </summary>
    /// <param name="cancellationToken">The CancellationToken</param>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> GetPaginatedAsync(CancellationToken cancellationToken)
    {
        try
        {
            var paginateFilter = new PaginateFilter(PerPage, CurrentPage);
            var paginatedResponse = await _mediator.Send(new GetChatPaginatedQuery(paginateFilter), cancellationToken);

            return Ok(ChatPaginatedResponse.From(paginatedResponse));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{Message}", ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Get the chat paginated using the vector search with the search parameter.
    /// </summary>
    /// <param name="search">The text to search</param>
    /// <param name="cancellationToken">The Cancellation Token</param>
    /// <returns></returns>
    [HttpGet("search")]
    public async Task<IActionResult> GetByVectorSearchAsync([FromQuery] string search, CancellationToken cancellationToken)
    {
        try
        {
            var response = new GetChatByVectorSearchQuery(new PaginateFilter(PerPage, CurrentPage), search);

            var paginatedResponse = await _mediator.Send(response, cancellationToken);

            return Ok(ChatPaginatedResponse.From(paginatedResponse));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{Message}", ex.Message);
            throw;
        }
    }
}
