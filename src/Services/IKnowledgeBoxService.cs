using KnowledgeBox.Structure.Models;

namespace KnowledgeBox.Structure.Services;

public interface IKnowledgeBoxService
{
    Task<KnowledgeBoxListResponse> GetAllKnowledgeBoxesAsync(string userId);
    Task<KnowledgeBoxResponse> GetKnowledgeBoxByIdAsync(string id, string userId);
    Task<KnowledgeBoxResponse> CreateKnowledgeBoxAsync(CreateKnowledgeBoxRequest request, string userId);
    Task<KnowledgeBoxResponse> UpdateKnowledgeBoxAsync(UpdateKnowledgeBoxRequest request, string userId);
    Task<DeleteKnowledgeBoxResponse> DeleteKnowledgeBoxAsync(string id, string userId);
    Task<KnowledgeBoxListResponse> SearchKnowledgeBoxesAsync(string? query, string? tags, string userId);
    Task<KnowledgeBoxListResponse> GetPublicKnowledgeBoxesAsync();
}