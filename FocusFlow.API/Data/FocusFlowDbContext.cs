using Microsoft.EntityFrameworkCore;
using FocusFlow.API.Models;

namespace FocusFlow.API.Data;

public class FocusFlowDbContext : DbContext
{
    public FocusFlowDbContext(DbContextOptions<FocusFlowDbContext> options) : base(options)
    {
    }

    public DbSet<TaskItem> Tasks { get; set; }
    public DbSet<NoteItem> Notes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<TaskItem>(entity =>
        {
            entity.HasKey(e => e.TaskID);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(1000);
            
            entity.Property(e => e.IsCompleted).IsRequired();
            
            entity.Property(e => e.DueDate).IsRequired(false);
            
            entity.Property(e => e.Priority)
                .HasConversion<int>()
                .IsRequired();
        });
        
        modelBuilder.Entity<NoteItem>(entity =>
        {
            entity.HasKey(e => e.NoteID);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Content).HasMaxLength(5000);
            
            entity.HasOne<TaskItem>()
                .WithMany()
                .HasForeignKey(e => e.LinkedTaskID)
                .OnDelete(DeleteBehavior.SetNull); 
        });
    }
}