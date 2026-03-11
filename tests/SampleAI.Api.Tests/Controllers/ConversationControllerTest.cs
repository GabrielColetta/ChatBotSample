using AutoFixture.Xunit3;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using SampleAI.Api.Models.Responses;
using SampleAI.Domain.Entities;
using SampleAI.Domain.Interfaces;
using SampleAI.Shared.Models;
using System.Net;
using System.Text.Json;

namespace SampleAI.Api.Tests.Controllers;

[Collection("Tests")]
public class ConversationControllerTest : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _options = new() { PropertyNameCaseInsensitive = true };

    public ConversationControllerTest(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _httpClient = _factory.CreateClient();
    }

    [Fact]
    public async Task GetConversationAsync_Should_ReturnEmptyData_When_NoConversationExistsInDatabase()
    {
        // Arrange
        var expectedData = new PaginatedResponse<ChatPaginatedResponse>([], 10, 0, 0);

        // Act
        var response = await _httpClient.GetAsync($"/api/conversation/{Guid.NewGuid()}", CancellationToken.None);
        var content = await response.Content.ReadAsStringAsync(CancellationToken.None);

        var actual = JsonSerializer.Deserialize<PaginatedResponse<ConversationResponse>>(content, _options);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        actual.Should().BeEquivalentTo(expectedData);
    }

    [Theory]
    [AutoData]
    public async Task GetConversationAsync_Should_ReturnDataWithCorrectProperties(ChatRole chatRole, string contentValue, float[] contentEmbeddings)
    {
        // Arrange
        using var scopedProvider = _factory.Services.CreateScope();
        var repository = scopedProvider.ServiceProvider.GetRequiredService<IChatRepository>();

        var chat = new Chat(contentValue, chatRole.ToString(), contentValue, contentEmbeddings);
        var expectedData = new PaginatedResponse<ConversationResponse>([new ConversationResponse(chat.Id.ToString(), chatRole.ToString(), chat.Date, contentValue)], 10, 0, 1);

        await repository.CreateAsync(chat, CancellationToken.None);

        // Act
        var response = await _httpClient.GetAsync($"/api/conversation/{chat.Id}", CancellationToken.None);
        var content = await response.Content.ReadAsStringAsync(CancellationToken.None);

        var actual = JsonSerializer.Deserialize<PaginatedResponse<ConversationResponse>>(content, _options);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        actual.Should().BeEquivalentTo(expectedData);
    }
}
