using Microsoft.EntityFrameworkCore;
using KnowledgeBox.Structure.Data;
using KnowledgeBox.Structure.Models;

namespace KnowledgeBox.Structure.Services;

public class KnowledgeBoxService : IKnowledgeBoxService
{
    private readonly KnowledgeBoxContext _context;

    public KnowledgeBoxService(KnowledgeBoxContext context)
    {
        _context = context;
    }

    public async Task<KnowledgeBoxListResponse> GetAllKnowledgeBoxesAsync()
    {
        try
        {
            var knowledgeBoxes = await _context.KnowledgeBoxes
                .OrderByDescending(kb => kb.UpdatedAt)
                .ToListAsync();

            return new KnowledgeBoxListResponse
            {
                Success = true,
                KnowledgeBoxes = knowledgeBoxes,
                TotalCount = knowledgeBoxes.Count
            };
        }
        catch (Exception ex)
        {
            return new KnowledgeBoxListResponse
            {
                Success = false,
                Message = "Failed to retrieve knowledge boxes: " + ex.Message
            };
        }
    }

    public async Task<KnowledgeBoxResponse> GetKnowledgeBoxByIdAsync(string id)
    {
        try
        {
            var knowledgeBox = await _context.KnowledgeBoxes
                .FirstOrDefaultAsync(kb => kb.Id == id);

            if (knowledgeBox == null)
            {
                return new KnowledgeBoxResponse
                {
                    Success = false,
                    Message = "Knowledge box not found"
                };
            }

            return new KnowledgeBoxResponse
            {
                Success = true,
                KnowledgeBox = knowledgeBox
            };
        }
        catch (Exception ex)
        {
            return new KnowledgeBoxResponse
            {
                Success = false,
                Message = "Failed to retrieve knowledge box: " + ex.Message
            };
        }
    }

    public async Task<KnowledgeBoxResponse> CreateKnowledgeBoxAsync(CreateKnowledgeBoxRequest request)
    {
        try
        {
            var knowledgeBox = new Models.KnowledgeBox
            {
                Id = Guid.NewGuid().ToString(),
                Title = request.Title,
                Topic = request.Topic,
                Content = request.Content ?? string.Empty,
                UserId = "anonymous", // Default user ID since no authentication
                IsPublic = request.IsPublic ?? false,
                Tags = request.Tags?.ToList() ?? new List<string>(),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            // Normalize tags (trim and make lowercase)
            knowledgeBox.Tags = knowledgeBox.Tags
                .Select(tag => tag.Trim().ToLowerInvariant())
                .Where(tag => !string.IsNullOrEmpty(tag))
                .Distinct()
                .ToList();

            _context.KnowledgeBoxes.Add(knowledgeBox);
            await _context.SaveChangesAsync();

            return new KnowledgeBoxResponse
            {
                Success = true,
                Message = "Knowledge box created successfully",
                KnowledgeBox = knowledgeBox
            };
        }
        catch (Exception ex)
        {
            return new KnowledgeBoxResponse
            {
                Success = false,
                Message = "Failed to create knowledge box: " + ex.Message
            };
        }
    }

    public async Task<KnowledgeBoxResponse> UpdateKnowledgeBoxAsync(UpdateKnowledgeBoxRequest request)
    {
        try
        {
            var knowledgeBox = await _context.KnowledgeBoxes
                .FirstOrDefaultAsync(kb => kb.Id == request.Id);

            if (knowledgeBox == null)
            {
                return new KnowledgeBoxResponse
                {
                    Success = false,
                    Message = "Knowledge box not found"
                };
            }

            // Update only provided fields
            if (request.Title != null)
                knowledgeBox.Title = request.Title;
            
            if (request.Topic != null)
                knowledgeBox.Topic = request.Topic;
            
            if (request.Content != null)
                knowledgeBox.Content = request.Content;
            
            if (request.IsPublic.HasValue)
                knowledgeBox.IsPublic = request.IsPublic.Value;
            
            if (request.Tags != null)
            {
                knowledgeBox.Tags = request.Tags
                    .Select(tag => tag.Trim().ToLowerInvariant())
                    .Where(tag => !string.IsNullOrEmpty(tag))
                    .Distinct()
                    .ToList();
            }

            knowledgeBox.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return new KnowledgeBoxResponse
            {
                Success = true,
                Message = "Knowledge box updated successfully",
                KnowledgeBox = knowledgeBox
            };
        }
        catch (Exception ex)
        {
            return new KnowledgeBoxResponse
            {
                Success = false,
                Message = "Failed to update knowledge box: " + ex.Message
            };
        }
    }

    public async Task<DeleteKnowledgeBoxResponse> DeleteKnowledgeBoxAsync(string id)
    {
        try
        {
            var knowledgeBox = await _context.KnowledgeBoxes
                .FirstOrDefaultAsync(kb => kb.Id == id);

            if (knowledgeBox == null)
            {
                return new DeleteKnowledgeBoxResponse
                {
                    Success = false,
                    Message = "Knowledge box not found"
                };
            }

            _context.KnowledgeBoxes.Remove(knowledgeBox);
            await _context.SaveChangesAsync();

            return new DeleteKnowledgeBoxResponse
            {
                Success = true,
                Message = "Knowledge box deleted successfully"
            };
        }
        catch (Exception ex)
        {
            return new DeleteKnowledgeBoxResponse
            {
                Success = false,
                Message = "Failed to delete knowledge box: " + ex.Message
            };
        }
    }

    public async Task<KnowledgeBoxListResponse> SearchKnowledgeBoxesAsync(string? query, string? tags)
    {
        try
        {
            var knowledgeBoxesQuery = _context.KnowledgeBoxes.AsQueryable();

            // Apply text search if query is provided
            if (!string.IsNullOrEmpty(query))
            {
                var searchTerm = query.ToLowerInvariant();
                knowledgeBoxesQuery = knowledgeBoxesQuery
                    .Where(kb => 
                        kb.Title.ToLower().Contains(searchTerm) ||
                        kb.Topic.ToLower().Contains(searchTerm) ||
                        kb.Content.ToLower().Contains(searchTerm));
            }

            // Apply tag filter if tags are provided
            if (!string.IsNullOrEmpty(tags))
            {
                var tagList = tags.Split(',')
                    .Select(tag => tag.Trim().ToLowerInvariant())
                    .Where(tag => !string.IsNullOrEmpty(tag))
                    .ToList();

                if (tagList.Any())
                {
                    knowledgeBoxesQuery = knowledgeBoxesQuery
                        .Where(kb => tagList.Any(tag => kb.Tags.Contains(tag)));
                }
            }

            var knowledgeBoxes = await knowledgeBoxesQuery
                .OrderByDescending(kb => kb.UpdatedAt)
                .ToListAsync();

            return new KnowledgeBoxListResponse
            {
                Success = true,
                KnowledgeBoxes = knowledgeBoxes,
                TotalCount = knowledgeBoxes.Count
            };
        }
        catch (Exception ex)
        {
            return new KnowledgeBoxListResponse
            {
                Success = false,
                Message = "Failed to search knowledge boxes: " + ex.Message
            };
        }
    }

    public async Task<KnowledgeBoxListResponse> GetPublicKnowledgeBoxesAsync()
    {
        try
        {
            var knowledgeBoxes = await _context.KnowledgeBoxes
                .Where(kb => kb.IsPublic)
                .OrderByDescending(kb => kb.UpdatedAt)
                .ToListAsync();

            return new KnowledgeBoxListResponse
            {
                Success = true,
                KnowledgeBoxes = knowledgeBoxes,
                TotalCount = knowledgeBoxes.Count
            };
        }
        catch (Exception ex)
        {
            return new KnowledgeBoxListResponse
            {
                Success = false,
                Message = "Failed to retrieve public knowledge boxes: " + ex.Message
            };
        }
    }
}