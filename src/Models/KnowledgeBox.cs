using System.ComponentModel.DataAnnotations;

namespace KnowledgeBox.Structure.Models;

public class KnowledgeBox
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    
    [Required]
    public string Title { get; set; } = string.Empty;
    
    [Required]
    public string Topic { get; set; } = string.Empty;
    
    public string Content { get; set; } = string.Empty;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    [Required]
    public string UserId { get; set; } = string.Empty;
    
    public bool IsPublic { get; set; } = false;
    
    public List<string> Tags { get; set; } = new();
    
    public List<string> Collaborators { get; set; } = new();
}