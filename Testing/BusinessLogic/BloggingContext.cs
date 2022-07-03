using System;
using Microsoft.EntityFrameworkCore;

namespace IQueryableInvokeReplacer.Testing.BusinessLogic;

public class BloggingContext : DbContext
{
    private readonly Action<BloggingContext, ModelBuilder>? _modelCustomizer;

    public BloggingContext()
    {
    }

    public BloggingContext(DbContextOptions<BloggingContext> options, Action<BloggingContext, ModelBuilder>? modelCustomizer = null)
        : base(options)
    {
        _modelCustomizer = modelCustomizer;
    }


    public DbSet<Blog> Blogs => Set<Blog>();
    public DbSet<Post> UrlResources => Set<Post>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Blog>()
            .HasKey(b => b.BlogId);

        modelBuilder.Entity<Post>()
            .HasAlternateKey(b => new { b.PostId, b.BlogId });

        modelBuilder.Entity<Post>().Property(e => e.UpdateTime).HasDefaultValue(DateTime.Now);

        if (_modelCustomizer is not null)
        {
            _modelCustomizer(this, modelBuilder);
        }
    }
}