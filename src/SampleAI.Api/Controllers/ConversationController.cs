using DnsClient.Internal;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SampleAI.Api.Models.Responses;
using SampleAI.Application.Contracts.Queries;
using SampleAI.Shared.Filters;

namespace SampleAI.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ConversationController : ControllerBase
{
    private const int CurrentPage = 0;
    private const int PerPage = 10;

    private readonly IMediator _mediator;
    private readonly ILogger<ConversationController> _logger;

    public ConversationController(IMediator mediator, ILogger<ConversationController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Get the conversation paginated by id.
    /// </summary>
    /// <param name="chatId">The chat unique identifier</param>
    /// <param name="cancellationToken">The Cancellation Token</param>
    /// <returns></returns>
    [HttpGet("{chatId}")]
    public async Task<IActionResult> GetConversationAsync([FromRoute] Guid chatId, CancellationToken cancellationToken)
    {
        try
        {
            var response = new GetConversationByIdQuery(new PaginateFilter(PerPage, CurrentPage), chatId);

            var paginatedResponse = await _mediator.Send(response, cancellationToken);

            return Ok(ConversationResponse.From(paginatedResponse));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{Message}", ex.Message);
            throw;
        }
    }
}
