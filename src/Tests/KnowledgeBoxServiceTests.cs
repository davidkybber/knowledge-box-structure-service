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
        var userId = "test-user-id";

        // Act
        var result = await _service.CreateKnowledgeBoxAsync(request, userId);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.KnowledgeBox);
        Assert.Equal(request.Title, result.KnowledgeBox.Title);
        Assert.Equal(request.Topic, result.KnowledgeBox.Topic);
        Assert.Equal(request.Content, result.KnowledgeBox.Content);
        Assert.Equal(userId, result.KnowledgeBox.UserId);
        Assert.Equal(2, result.KnowledgeBox.Tags.Count);
        Assert.Contains("test", result.KnowledgeBox.Tags);
        Assert.Contains("knowledge", result.KnowledgeBox.Tags);
    }

    [Fact]
    public async Task GetKnowledgeBoxByIdAsync_ExistingKnowledgeBox_ReturnsSuccess()
    {
        // Arrange
        var userId = "test-user-id";
        var knowledgeBox = new Models.KnowledgeBox
        {
            Id = Guid.NewGuid().ToString(),
            Title = "Test Knowledge Box",
            Topic = "Test Topic",
            Content = "Test Content",
            UserId = userId,
            IsPublic = false,
            Tags = new List<string> { "test", "knowledge" }
        };

        _context.KnowledgeBoxes.Add(knowledgeBox);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.GetKnowledgeBoxByIdAsync(knowledgeBox.Id, userId);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.KnowledgeBox);
        Assert.Equal(knowledgeBox.Id, result.KnowledgeBox.Id);
        Assert.Equal(knowledgeBox.Title, result.KnowledgeBox.Title);
    }

    [Fact]
    public async Task GetKnowledgeBoxByIdAsync_NonExistentKnowledgeBox_ReturnsFailure()
    {
        // Arrange
        var userId = "test-user-id";
        var nonExistentId = Guid.NewGuid().ToString();

        // Act
        var result = await _service.GetKnowledgeBoxByIdAsync(nonExistentId, userId);

        // Assert
        Assert.False(result.Success);
        Assert.Null(result.KnowledgeBox);
        Assert.Equal("Knowledge box not found or access denied", result.Message);
    }

    [Fact]
    public async Task GetKnowledgeBoxByIdAsync_PublicKnowledgeBox_DifferentUser_ReturnsSuccess()
    {
        // Arrange
        var ownerId = "owner-user-id";
        var requesterId = "requester-user-id";
        var knowledgeBox = new Models.KnowledgeBox
        {
            Id = Guid.NewGuid().ToString(),
            Title = "Public Knowledge Box",
            Topic = "Public Topic",
            Content = "Public Content",
            UserId = ownerId,
            IsPublic = true,
            Tags = new List<string> { "public", "knowledge" }
        };

        _context.KnowledgeBoxes.Add(knowledgeBox);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.GetKnowledgeBoxByIdAsync(knowledgeBox.Id, requesterId);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.KnowledgeBox);
        Assert.Equal(knowledgeBox.Id, result.KnowledgeBox.Id);
    }

    [Fact]
    public async Task UpdateKnowledgeBoxAsync_ValidRequest_ReturnsSuccess()
    {
        // Arrange
        var userId = "test-user-id";
        var knowledgeBox = new Models.KnowledgeBox
        {
            Id = Guid.NewGuid().ToString(),
            Title = "Original Title",
            Topic = "Original Topic",
            Content = "Original Content",
            UserId = userId,
            IsPublic = false,
            Tags = new List<string> { "original" }
        };

        _context.KnowledgeBoxes.Add(knowledgeBox);
        await _context.SaveChangesAsync();

        var updateRequest = new UpdateKnowledgeBoxRequest
        {
            Id = knowledgeBox.Id,
            Title = "Updated Title",
            Content = "Updated Content",
            Tags = new[] { "updated", "knowledge" }
        };

        // Act
        var result = await _service.UpdateKnowledgeBoxAsync(updateRequest, userId);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.KnowledgeBox);
        Assert.Equal("Updated Title", result.KnowledgeBox.Title);
        Assert.Equal("Updated Content", result.KnowledgeBox.Content);
        Assert.Equal("Original Topic", result.KnowledgeBox.Topic); // Should remain unchanged
        Assert.Equal(2, result.KnowledgeBox.Tags.Count);
        Assert.Contains("updated", result.KnowledgeBox.Tags);
        Assert.Contains("knowledge", result.KnowledgeBox.Tags);
    }

    [Fact]
    public async Task DeleteKnowledgeBoxAsync_ExistingKnowledgeBox_ReturnsSuccess()
    {
        // Arrange
        var userId = "test-user-id";
        var knowledgeBox = new Models.KnowledgeBox
        {
            Id = Guid.NewGuid().ToString(),
            Title = "Test Knowledge Box",
            Topic = "Test Topic",
            Content = "Test Content",
            UserId = userId,
            IsPublic = false,
            Tags = new List<string> { "test" }
        };

        _context.KnowledgeBoxes.Add(knowledgeBox);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.DeleteKnowledgeBoxAsync(knowledgeBox.Id, userId);

        // Assert
        Assert.True(result.Success);
        Assert.Equal("Knowledge box deleted successfully", result.Message);

        // Verify it's actually deleted
        var deletedKnowledgeBox = await _context.KnowledgeBoxes.FindAsync(knowledgeBox.Id);
        Assert.Null(deletedKnowledgeBox);
    }

    [Fact]
    public async Task SearchKnowledgeBoxesAsync_WithQuery_ReturnsMatchingResults()
    {
        // Arrange
        var userId = "test-user-id";
        var knowledgeBox1 = new Models.KnowledgeBox
        {
            Id = Guid.NewGuid().ToString(),
            Title = "Programming in C#",
            Topic = "Software Development",
            Content = "Learning C# programming language",
            UserId = userId,
            IsPublic = false,
            Tags = new List<string> { "programming", "csharp" }
        };

        var knowledgeBox2 = new Models.KnowledgeBox
        {
            Id = Guid.NewGuid().ToString(),
            Title = "Machine Learning Basics",
            Topic = "Artificial Intelligence",
            Content = "Introduction to machine learning",
            UserId = userId,
            IsPublic = false,
            Tags = new List<string> { "ml", "ai" }
        };

        _context.KnowledgeBoxes.AddRange(knowledgeBox1, knowledgeBox2);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.SearchKnowledgeBoxesAsync("programming", null, userId);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.KnowledgeBoxes);
        Assert.Single(result.KnowledgeBoxes);
        Assert.Equal(knowledgeBox1.Id, result.KnowledgeBoxes[0].Id);
    }

    [Fact]
    public async Task SearchKnowledgeBoxesAsync_WithTags_ReturnsMatchingResults()
    {
        // Arrange
        var userId = "test-user-id";
        var knowledgeBox1 = new Models.KnowledgeBox
        {
            Id = Guid.NewGuid().ToString(),
            Title = "Programming in C#",
            Topic = "Software Development",
            Content = "Learning C# programming language",
            UserId = userId,
            IsPublic = false,
            Tags = new List<string> { "programming", "csharp" }
        };

        var knowledgeBox2 = new Models.KnowledgeBox
        {
            Id = Guid.NewGuid().ToString(),
            Title = "Machine Learning Basics",
            Topic = "Artificial Intelligence",
            Content = "Introduction to machine learning",
            UserId = userId,
            IsPublic = false,
            Tags = new List<string> { "ml", "ai" }
        };

        _context.KnowledgeBoxes.AddRange(knowledgeBox1, knowledgeBox2);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.SearchKnowledgeBoxesAsync(null, "programming,csharp", userId);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.KnowledgeBoxes);
        Assert.Single(result.KnowledgeBoxes);
        Assert.Equal(knowledgeBox1.Id, result.KnowledgeBoxes[0].Id);
    }

    [Fact]
    public async Task GetPublicKnowledgeBoxesAsync_ReturnsOnlyPublicKnowledgeBoxes()
    {
        // Arrange
        var userId1 = "user1";
        var userId2 = "user2";
        
        var publicKnowledgeBox = new Models.KnowledgeBox
        {
            Id = Guid.NewGuid().ToString(),
            Title = "Public Knowledge Box",
            Topic = "Public Topic",
            Content = "Public content",
            UserId = userId1,
            IsPublic = true,
            Tags = new List<string> { "public" }
        };

        var privateKnowledgeBox = new Models.KnowledgeBox
        {
            Id = Guid.NewGuid().ToString(),
            Title = "Private Knowledge Box",
            Topic = "Private Topic",
            Content = "Private content",
            UserId = userId2,
            IsPublic = false,
            Tags = new List<string> { "private" }
        };

        _context.KnowledgeBoxes.AddRange(publicKnowledgeBox, privateKnowledgeBox);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.GetPublicKnowledgeBoxesAsync();

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.KnowledgeBoxes);
        Assert.Single(result.KnowledgeBoxes);
        Assert.Equal(publicKnowledgeBox.Id, result.KnowledgeBoxes[0].Id);
        Assert.True(result.KnowledgeBoxes[0].IsPublic);
    }

    [Fact]
    public async Task GetAllKnowledgeBoxesAsync_ReturnsUserKnowledgeBoxes()
    {
        // Arrange
        var userId1 = "user1";
        var userId2 = "user2";

        var knowledgeBox1 = new Models.KnowledgeBox
        {
            Id = Guid.NewGuid().ToString(),
            Title = "User1 Knowledge Box",
            Topic = "User1 Topic",
            Content = "User1 content",
            UserId = userId1,
            IsPublic = false,
            Tags = new List<string> { "user1" }
        };

        var knowledgeBox2 = new Models.KnowledgeBox
        {
            Id = Guid.NewGuid().ToString(),
            Title = "User2 Knowledge Box",
            Topic = "User2 Topic",
            Content = "User2 content",
            UserId = userId2,
            IsPublic = false,
            Tags = new List<string> { "user2" }
        };

        _context.KnowledgeBoxes.AddRange(knowledgeBox1, knowledgeBox2);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.GetAllKnowledgeBoxesAsync(userId1);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.KnowledgeBoxes);
        Assert.Single(result.KnowledgeBoxes);
        Assert.Equal(knowledgeBox1.Id, result.KnowledgeBoxes[0].Id);
        Assert.Equal(userId1, result.KnowledgeBoxes[0].UserId);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}