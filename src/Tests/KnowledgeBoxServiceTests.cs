using Microsoft.EntityFrameworkCore;
using KnowledgeBox.Structure.Data;
using KnowledgeBox.Structure.Models;
using KnowledgeBox.Structure.Services;
using Xunit;

namespace KnowledgeBox.Structure.Tests;

public class KnowledgeBoxServiceTests : IDisposable
{
    private readonly KnowledgeBoxContext _context;
    private readonly KnowledgeBoxService _service;

    public KnowledgeBoxServiceTests()
    {
        var options = new DbContextOptionsBuilder<KnowledgeBoxContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new KnowledgeBoxContext(options);
        _service = new KnowledgeBoxService(_context);
    }

    [Fact]
    public async Task CreateKnowledgeBoxAsync_ValidRequest_ReturnsSuccess()
    {
        // Arrange
        var request = new CreateKnowledgeBoxRequest
        {
            Title = "Test Knowledge Box",
            Topic = "Test Topic",
            Content = "Test Content",
            IsPublic = false,
            Tags = new[] { "test", "knowledge" }
        };

        // Act
        var result = await _service.CreateKnowledgeBoxAsync(request);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.KnowledgeBox);
        Assert.Equal(request.Title, result.KnowledgeBox.Title);
        Assert.Equal(request.Topic, result.KnowledgeBox.Topic);
        Assert.Equal(request.Content, result.KnowledgeBox.Content);
        Assert.Equal("anonymous", result.KnowledgeBox.UserId);
        Assert.Equal(2, result.KnowledgeBox.Tags.Count);
        Assert.Contains("test", result.KnowledgeBox.Tags);
        Assert.Contains("knowledge", result.KnowledgeBox.Tags);
    }

    [Fact]
    public async Task GetKnowledgeBoxByIdAsync_ExistingKnowledgeBox_ReturnsSuccess()
    {
        // Arrange
        var knowledgeBox = new Models.KnowledgeBox
        {
            Id = "test-id",
            Title = "Test Knowledge Box",
            Topic = "Test Topic",
            Content = "Test Content",
            UserId = "test-user-id",
            IsPublic = false,
            Tags = new List<string> { "test" },
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.KnowledgeBoxes.Add(knowledgeBox);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.GetKnowledgeBoxByIdAsync("test-id");

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.KnowledgeBox);
        Assert.Equal("test-id", result.KnowledgeBox.Id);
    }

    [Fact]
    public async Task GetKnowledgeBoxByIdAsync_NonExistentKnowledgeBox_ReturnsFailure()
    {
        // Act
        var result = await _service.GetKnowledgeBoxByIdAsync("non-existent-id");

        // Assert
        Assert.False(result.Success);
        Assert.Null(result.KnowledgeBox);
    }

    [Fact]
    public async Task GetKnowledgeBoxByIdAsync_PublicKnowledgeBox_ReturnsSuccess()
    {
        // Arrange
        var knowledgeBox = new Models.KnowledgeBox
        {
            Id = "public-id",
            Title = "Public Knowledge Box",
            Topic = "Public Topic",
            Content = "Public Content",
            UserId = "other-user-id",
            IsPublic = true,
            Tags = new List<string> { "public" },
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.KnowledgeBoxes.Add(knowledgeBox);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.GetKnowledgeBoxByIdAsync("public-id");

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.KnowledgeBox);
        Assert.Equal("public-id", result.KnowledgeBox.Id);
    }

    [Fact]
    public async Task UpdateKnowledgeBoxAsync_ValidRequest_ReturnsSuccess()
    {
        // Arrange
        var knowledgeBox = new Models.KnowledgeBox
        {
            Id = "test-id",
            Title = "Original Title",
            Topic = "Original Topic",
            Content = "Original Content",
            UserId = "test-user-id",
            IsPublic = false,
            Tags = new List<string> { "original" },
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.KnowledgeBoxes.Add(knowledgeBox);
        await _context.SaveChangesAsync();

        var request = new UpdateKnowledgeBoxRequest
        {
            Id = "test-id",
            Title = "Updated Title",
            Topic = "Updated Topic",
            Content = "Updated Content",
            IsPublic = true,
            Tags = new[] { "updated", "test" }
        };

        // Act
        var result = await _service.UpdateKnowledgeBoxAsync(request);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.KnowledgeBox);
        Assert.Equal("Updated Title", result.KnowledgeBox.Title);
        Assert.Equal("Updated Topic", result.KnowledgeBox.Topic);
        Assert.Equal("Updated Content", result.KnowledgeBox.Content);
        Assert.True(result.KnowledgeBox.IsPublic);
        Assert.Equal(2, result.KnowledgeBox.Tags.Count);
        Assert.Contains("updated", result.KnowledgeBox.Tags);
        Assert.Contains("test", result.KnowledgeBox.Tags);
    }

    [Fact]
    public async Task DeleteKnowledgeBoxAsync_ExistingKnowledgeBox_ReturnsSuccess()
    {
        // Arrange
        var knowledgeBox = new Models.KnowledgeBox
        {
            Id = "test-id",
            Title = "Test Knowledge Box",
            Topic = "Test Topic",
            Content = "Test Content",
            UserId = "test-user-id",
            IsPublic = false,
            Tags = new List<string> { "test" },
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.KnowledgeBoxes.Add(knowledgeBox);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.DeleteKnowledgeBoxAsync("test-id");

        // Assert
        Assert.True(result.Success);
        
        // Verify the knowledge box was actually deleted
        var deletedKnowledgeBox = await _context.KnowledgeBoxes.FindAsync("test-id");
        Assert.Null(deletedKnowledgeBox);
    }

    [Fact]
    public async Task SearchKnowledgeBoxesAsync_WithQuery_ReturnsMatchingResults()
    {
        // Arrange
        var knowledgeBox1 = new Models.KnowledgeBox
        {
            Id = "test-id-1",
            Title = "Machine Learning Basics",
            Topic = "AI",
            Content = "Introduction to machine learning",
            UserId = "test-user-id",
            IsPublic = true,
            Tags = new List<string> { "ai", "ml" },
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var knowledgeBox2 = new Models.KnowledgeBox
        {
            Id = "test-id-2",
            Title = "Web Development",
            Topic = "Programming",
            Content = "HTML, CSS, JavaScript basics",
            UserId = "test-user-id",
            IsPublic = false,
            Tags = new List<string> { "web", "programming" },
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.KnowledgeBoxes.AddRange(knowledgeBox1, knowledgeBox2);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.SearchKnowledgeBoxesAsync("machine", null);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(1, result.TotalCount);
        Assert.Contains(result.KnowledgeBoxes, kb => kb.Title == "Machine Learning Basics");
    }

    [Fact]
    public async Task SearchKnowledgeBoxesAsync_WithTags_ReturnsMatchingResults()
    {
        // Arrange
        var knowledgeBox1 = new Models.KnowledgeBox
        {
            Id = "test-id-1",
            Title = "Machine Learning Basics",
            Topic = "AI",
            Content = "Introduction to machine learning",
            UserId = "test-user-id",
            IsPublic = true,
            Tags = new List<string> { "ai", "ml" },
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var knowledgeBox2 = new Models.KnowledgeBox
        {
            Id = "test-id-2",
            Title = "Web Development",
            Topic = "Programming",
            Content = "HTML, CSS, JavaScript basics",
            UserId = "test-user-id",
            IsPublic = false,
            Tags = new List<string> { "web", "programming" },
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.KnowledgeBoxes.AddRange(knowledgeBox1, knowledgeBox2);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.SearchKnowledgeBoxesAsync(null, "ai");

        // Assert
        Assert.True(result.Success);
        Assert.Equal(1, result.TotalCount);
        Assert.Contains(result.KnowledgeBoxes, kb => kb.Title == "Machine Learning Basics");
    }

    [Fact]
    public async Task GetPublicKnowledgeBoxesAsync_ReturnsOnlyPublicKnowledgeBoxes()
    {
        // Arrange
        var publicKnowledgeBox = new Models.KnowledgeBox
        {
            Id = "public-id",
            Title = "Public Knowledge Box",
            Topic = "Public Topic",
            Content = "Public Content",
            UserId = "user-1",
            IsPublic = true,
            Tags = new List<string> { "public" },
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var privateKnowledgeBox = new Models.KnowledgeBox
        {
            Id = "private-id",
            Title = "Private Knowledge Box",
            Topic = "Private Topic",
            Content = "Private Content",
            UserId = "user-2",
            IsPublic = false,
            Tags = new List<string> { "private" },
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.KnowledgeBoxes.AddRange(publicKnowledgeBox, privateKnowledgeBox);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.GetPublicKnowledgeBoxesAsync();

        // Assert
        Assert.True(result.Success);
        Assert.Equal(1, result.TotalCount);
        Assert.Contains(result.KnowledgeBoxes, kb => kb.Id == "public-id");
        Assert.DoesNotContain(result.KnowledgeBoxes, kb => kb.Id == "private-id");
    }

    [Fact]
    public async Task GetAllKnowledgeBoxesAsync_ReturnsAllKnowledgeBoxes()
    {
        // Arrange
        var knowledgeBox1 = new Models.KnowledgeBox
        {
            Id = "test-id-1",
            Title = "Knowledge Box 1",
            Topic = "Topic 1",
            Content = "Content 1",
            UserId = "user-1",
            IsPublic = true,
            Tags = new List<string> { "tag1" },
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var knowledgeBox2 = new Models.KnowledgeBox
        {
            Id = "test-id-2",
            Title = "Knowledge Box 2",
            Topic = "Topic 2",
            Content = "Content 2",
            UserId = "user-2",
            IsPublic = false,
            Tags = new List<string> { "tag2" },
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.KnowledgeBoxes.AddRange(knowledgeBox1, knowledgeBox2);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.GetAllKnowledgeBoxesAsync();

        // Assert
        Assert.True(result.Success);
        Assert.Equal(2, result.TotalCount);
        Assert.Contains(result.KnowledgeBoxes, kb => kb.Id == "test-id-1");
        Assert.Contains(result.KnowledgeBoxes, kb => kb.Id == "test-id-2");
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}