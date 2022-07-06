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
    public DbSet<Post> Posts => Set<Post>();
    public DbSet<Comment> Comments => Set<Comment>();
    public DbSet<User> Users => Set<User>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        modelBuilder.Entity<Comment>()
            .HasKey(c => new { c.PostId, c.index });

        modelBuilder.Entity<Post>()
            .Property(e => e.UpdateTime).HasDefaultValue(DateTime.Now);

        if (_modelCustomizer is not null)
        {
            _modelCustomizer(this, modelBuilder);
        }
    }
}