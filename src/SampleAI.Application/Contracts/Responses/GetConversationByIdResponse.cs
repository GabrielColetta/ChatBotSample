namespace SampleAI.Application.Contracts.Responses;

public record GetConversationByIdResponse(Guid Id, string ChatRole, DateTime Date, string Content);