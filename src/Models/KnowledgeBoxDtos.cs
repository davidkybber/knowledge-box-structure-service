using System.ComponentModel.DataAnnotations;

namespace KnowledgeBox.Structure.Models;

public class CreateKnowledgeBoxRequest
{
    [Required]
    public string Title { get; set; } = string.Empty;
    
    [Required]
    public string Topic { get; set; } = string.Empty;
    
    public string? Content { get; set; }
    
    public bool? IsPublic { get; set; }
    
    public string[]? Tags { get; set; }
}

public class UpdateKnowledgeBoxRequest
{
    [Required]
    public string Id { get; set; } = string.Empty;
    
    public string? Title { get; set; }
    
    public string? Topic { get; set; }
    
    public string? Content { get; set; }
    
    public bool? IsPublic { get; set; }
    
    public string[]? Tags { get; set; }
}

public class KnowledgeBoxResponse
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public KnowledgeBox? KnowledgeBox { get; set; }
}

public class KnowledgeBoxListResponse
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public List<KnowledgeBox>? KnowledgeBoxes { get; set; }
    public int? TotalCount { get; set; }
}

public class DeleteKnowledgeBoxResponse
{
    public bool Success { get; set; }
    public string? Message { get; set; }
}

public class ErrorResponse
{
    public bool Success { get; set; } = false;
    public string Message { get; set; } = string.Empty;
    public string? Error { get; set; }
}