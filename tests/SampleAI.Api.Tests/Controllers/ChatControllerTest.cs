using AutoFixture.Xunit3;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using SampleAI.Api.Models.Responses;
using SampleAI.Application.Services;
using SampleAI.Domain.Entities;
using SampleAI.Domain.Interfaces;
using SampleAI.Shared.Models;
using System.Net;
using System.Text.Json;

namespace SampleAI.Api.Tests.Controllers;

[Collection("Tests")]
public class ChatControllerTest : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private const string RequestUri = "/api/chat";

    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _options = new() { PropertyNameCaseInsensitive = true };

    public ChatControllerTest(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _httpClient = _factory.CreateClient();
    }

    [Theory]
    [AutoData]
    public async Task GetPaginatedAsync_Should_ReturnChatDataWithCorrectProperties(string title, DateTime date)
    {
        // Arrange
        var formattedDate = new DateTime(date.Year, date.Month, date.Day, date.Hour, date.Minute, date.Second, DateTimeKind.Utc);

        using var scopedProvider = _factory.Services.CreateScope();
        var repository = scopedProvider.ServiceProvider.GetRequiredService<IChatRepository>();
        await InsertChat(repository, title, formattedDate);

        // Act
        var response = await _httpClient.GetAsync(RequestUri, CancellationToken.None);
        var content = await response.Content.ReadAsStringAsync(CancellationToken.None);

        var actual = JsonSerializer.Deserialize<PaginatedResponse<ChatPaginatedResponse>>(content, _options);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        actual!.Data.Should().Contain(x => x.Title == title && x.Date == formattedDate);
    }

    [Theory]
    [AutoData]
    public async Task GetPaginatedAsync_Should_ReturnDataOrderedByDateDescending(string title, DateTime date)
    {
        // Arrange
        using var scopedProvider = _factory.Services.CreateScope();
        var repository = scopedProvider.ServiceProvider.GetRequiredService<IChatRepository>();

        var now = DateTime.UtcNow;
        await InsertChat(repository, title, date.AddDays(-1));
        await InsertChat(repository, title, date);
        await InsertChat(repository, title, date.AddHours(1));

        // Act
        var response = await _httpClient.GetAsync(RequestUri, CancellationToken.None);
        var content = await response.Content.ReadAsStringAsync(CancellationToken.None);

        var actual = JsonSerializer.Deserialize<PaginatedResponse<ChatPaginatedResponse>>(content, _options);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        actual!.Data.Should().BeInDescendingOrder(x => x.Date);
    }

    [Theory]
    [AutoData]
    public async Task GetByVectorSearchAsync_Should_ReturnDataWithCorrectProperties(ChatRole chatRole, string contentValue)
    {
        // Arrange
        using var scopedProvider = _factory.Services.CreateScope();

        var embeddingService = scopedProvider.ServiceProvider.GetRequiredService<IEmbeddingService>();
        var embeddings = await embeddingService.GetEmbeddingFromModelAsync(contentValue, CancellationToken.None);

        var chat = new Chat(contentValue, chatRole.ToString(), contentValue, embeddings);
        var expectedData = new ChatPaginatedResponse(chat.Id.ToString(), contentValue, chat.Date);

        var repository = scopedProvider.ServiceProvider.GetRequiredService<IChatRepository>();
        await repository.CreateAsync(chat, CancellationToken.None);

        //Required to ensure the data is indexed before searching with the filter.
        await Task.Delay(2000, CancellationToken.None);

        var filter = string.Concat(contentValue.AsSpan(0, contentValue.Length - 1), "D");

        // Act
        var response = await _httpClient.GetAsync($"/api/chat/search?search={filter}", CancellationToken.None);
        var content = await response.Content.ReadAsStringAsync(CancellationToken.None);

        var actual = JsonSerializer.Deserialize<PaginatedResponse<ChatPaginatedResponse>>(content, _options);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        actual!.Data.Should().ContainEquivalentOf(expectedData);
    }

    private static async Task InsertChat(IChatRepository repository, string title, DateTime date)
    {
        var chat = new Chat(title, "user", "Test content", []) { Date = date };

        await repository.CreateAsync(chat, CancellationToken.None);
    }
}
