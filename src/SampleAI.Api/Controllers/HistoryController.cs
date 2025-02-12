using Microsoft.AspNetCore.Mvc;
using SampleAI.Api.Models.Requests;
using SampleAI.Api.Models.Responses;
using SampleAI.Shared.Filters;
using SampleAI.Shared.Interfaces;
using SampleAI.Shared.Models;

namespace SampleAI.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class HistoryController : ControllerBase
{
    private const int CurrentPage = 0;
    private const int PerPage = 5;

    private readonly IDatabaseContext _databaseContext;
    private readonly ILogger<HistoryController> _logger;

    public HistoryController(IDatabaseContext databaseContext, ILogger<HistoryController> logger)
    {
        _databaseContext = databaseContext;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetPaginatedAsync([FromQuery]GetHistoryRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var paginateFilter = new PaginateFilters<ChatHistoryModel>(
                    request.PerPage,
                    request.CurrentPage, 
                    sortByDesc: x => x.Date,
                    groupBy: x => x.ConversationId,
                    filterBy: x => x.ChatRole == "user");
            var paginatedResponse = await _databaseContext.GetSamplePaginatedAsync(ChatHistoryModel.DocumentName, paginateFilter, cancellationToken);

            return Ok(HistoryResponse.From(paginatedResponse));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{Message}", ex.Message);
            throw;
        } 
    }

    [HttpGet("{conversationId}")]
    public async Task<IActionResult> GetByIdAsync([FromRoute] string conversationId, CancellationToken cancellationToken)
    {
        try
        {
            var paginateFilter = new PaginateFilters<ChatHistoryModel>(
                    PerPage,
                    CurrentPage,
                    sortByDesc: x => x,
                    filterBy: x => x.ConversationId == conversationId);
            var paginatedResponse = await _databaseContext.GetPaginatedAsync(ChatHistoryModel.DocumentName, paginateFilter, cancellationToken);

            return Ok(HistoryResponse.From(paginatedResponse));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{Message}", ex.Message);
            throw;
        }
    }
}
