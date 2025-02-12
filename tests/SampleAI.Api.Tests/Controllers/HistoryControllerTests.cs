using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using SampleAI.Api.Models.Responses;
using SampleAI.Shared.Interfaces;
using SampleAI.Shared.Models;
using System.Net.Http.Json;

namespace SampleAI.Api.Tests.Controllers;

public class HistoryControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public HistoryControllerTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Should_ReturnEmptyOk_When_GetHistoryAsync()
    {
        // Given
        var client = _factory.CreateClient();
        var expectedPaginatedResponse = new PaginatedResponse<HistoryResponse>([], 20, 0, 0);

        // When
        var response = await client.GetAsync("/api/history?currentPage=0&perPage=20");

        // Then
        response.EnsureSuccessStatusCode();
        var paginatedResponse = await response.Content.ReadFromJsonAsync<PaginatedResponse<HistoryResponse>>();
        paginatedResponse.Should().BeEquivalentTo(expectedPaginatedResponse);
    }

    [Theory]
    [AutoData]
    public async Task Should_ReturnOk_When_GetHistoryAsync(string content, Guid conversationId, DateTime date)
    {
        // Given
        var dateTimeOnly = date.Date;
        const string userRole = "user";
        var client = _factory.CreateClient();

        await using var scoped = _factory.Services.CreateAsyncScope();
        var mongoClient = scoped.ServiceProvider.GetRequiredService<IDatabaseContext>();
        await mongoClient.InsertAsync(ChatHistoryModel.DocumentName, new ChatHistoryModel(userRole, content, conversationId.ToString(), dateTimeOnly), CancellationToken.None);
        await mongoClient.InsertAsync(ChatHistoryModel.DocumentName, new ChatHistoryModel("assistant", content, conversationId.ToString(), dateTimeOnly), CancellationToken.None);

        var expectedPaginatedResponse = new PaginatedResponse<HistoryResponse>([new HistoryResponse(conversationId.ToString(), userRole, dateTimeOnly.ToUniversalTime(), content)], 20, 0, 1);

        // When 
        var response = await client.GetAsync("/api/history?currentPage=0&perPage=20");

        // Then
        response.EnsureSuccessStatusCode();
        var paginatedResponse = await response.Content.ReadFromJsonAsync<PaginatedResponse<HistoryResponse>>();
        paginatedResponse.Should().BeEquivalentTo(expectedPaginatedResponse);
    }

    [Theory]
    [AutoData]
    public async Task Should_ReturnEmptyOk_When_GetByIdAsync(Guid conversationId)
    {
        // Given
        var client = _factory.CreateClient();

        var expectedResponse = Enumerable.Empty<HistoryResponse>();

        // When
        var response = await client.GetAsync($"/api/history/{conversationId}");

        // Then
        response.EnsureSuccessStatusCode();
        var actualResponse = await response.Content.ReadFromJsonAsync<IEnumerable<HistoryResponse>>();
        actualResponse.Should().BeEquivalentTo(expectedResponse);
    }

    [Theory]
    [AutoData]
    public async Task Should_ReturnOk_When_GetByIdAsync(string chatRole, string content, Guid conversationId, Guid randomConversationId, DateTime date)
    {
        // Given
        var dateTimeOnly = date.Date;
        var client = _factory.CreateClient();

        await using var scoped = _factory.Services.CreateAsyncScope();
        var mongoClient = scoped.ServiceProvider.GetRequiredService<IDatabaseContext>();
        await mongoClient.InsertAsync(ChatHistoryModel.DocumentName, new ChatHistoryModel(chatRole, content, conversationId.ToString(), dateTimeOnly), CancellationToken.None);
        await mongoClient.InsertAsync(ChatHistoryModel.DocumentName, new ChatHistoryModel("chatRole", content, randomConversationId.ToString(), dateTimeOnly), CancellationToken.None);

        var expectedPaginatedResponse = new List<HistoryResponse>(1)
        {
            new(conversationId.ToString(), chatRole, dateTimeOnly.ToUniversalTime(), content)
        };

        // When 
        var response = await client.GetAsync($"/api/history/{conversationId}");

        // Then
        response.EnsureSuccessStatusCode();
        var paginatedResponse = await response.Content.ReadFromJsonAsync<IEnumerable<HistoryResponse>>();
        paginatedResponse.Should().BeEquivalentTo(expectedPaginatedResponse);
    }
}
