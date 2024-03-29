﻿using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Data.Common;
using Xunit.Abstractions;

namespace H.EFCore.Extensions.Test;

public class BloggingTestBase : IDisposable
{
    private readonly DbConnection _connection;
    private readonly DbContextOptions<BloggingContext> _contextOptions;
    private readonly ITestOutputHelper _testOutputHelper;

    protected BloggingContext CreateContext()
    {
        return new(_contextOptions);
    }

    public BloggingTestBase(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
        _connection = new SqliteConnection("Filename=:memory:");
        _connection.Open();

        _contextOptions = new DbContextOptionsBuilder<BloggingContext>()
            .UseSqlite(_connection)
#if DEBUG
            .LogTo(_testOutputHelper.WriteLine, new[] { RelationalEventId.CommandExecuting })
            .EnableSensitiveDataLogging()
#endif
            .Options;

        // Create the schema and seed some data
        using var context = new BloggingContext(_contextOptions);

        bool a = context.Database.EnsureCreated();

        context.AddRange(
            new Blog { BlogId = 1, Name = "Blog1", Url = "http://blog1.com" },
            new Blog { BlogId = 2, Name = "Blog2", Url = "http://blog2.com" }
            );
        context.AddRange(
            new Post { BlogId = 1, PostId = 1 },
            new Post { BlogId = 1, PostId = 2 });
        context.SaveChanges();
    }

    public void Dispose()
    {
        _connection.Dispose();
    }
}
