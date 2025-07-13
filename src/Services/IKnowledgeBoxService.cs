using KnowledgeBox.Structure.Models;

namespace KnowledgeBox.Structure.Services;

public interface IKnowledgeBoxService
{
    Task<KnowledgeBoxListResponse> GetAllKnowledgeBoxesAsync();
    Task<KnowledgeBoxResponse> GetKnowledgeBoxByIdAsync(string id);
    Task<KnowledgeBoxResponse> CreateKnowledgeBoxAsync(CreateKnowledgeBoxRequest request);
    Task<KnowledgeBoxResponse> UpdateKnowledgeBoxAsync(UpdateKnowledgeBoxRequest request);
    Task<DeleteKnowledgeBoxResponse> DeleteKnowledgeBoxAsync(string id);
    Task<KnowledgeBoxListResponse> SearchKnowledgeBoxesAsync(string? query, string? tags);
    Task<KnowledgeBoxListResponse> GetPublicKnowledgeBoxesAsync();
}