using Microsoft.EntityFrameworkCore;
using KnowledgeBox.Structure.Models;
using System.Text.Json;

namespace KnowledgeBox.Structure.Data;

public class KnowledgeBoxContext : DbContext
{
    public KnowledgeBoxContext(DbContextOptions<KnowledgeBoxContext> options) : base(options)
    {
    }

    public DbSet<Models.KnowledgeBox> KnowledgeBoxes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure KnowledgeBox entity
        modelBuilder.Entity<Models.KnowledgeBox>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Topic).IsRequired().HasMaxLength(200);
            entity.Property(e => e.UserId).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt).IsRequired();
            
            // Configure Tags as JSON column
            entity.Property(e => e.Tags)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null!),
                    v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions)null!) ?? new List<string>());

            // Configure Collaborators as JSON column
            entity.Property(e => e.Collaborators)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null!),
                    v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions)null!) ?? new List<string>());
        });
    }
}