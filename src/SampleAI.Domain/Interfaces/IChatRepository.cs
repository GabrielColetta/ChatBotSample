using SampleAI.Domain.Entities;
using SampleAI.Shared.Interfaces;

namespace SampleAI.Domain.Interfaces;

public interface IChatRepository : ICreatableRepository<Chat>,
    IPaginatable<Chat>,
    IGettableByIdRepository<Chat>;
