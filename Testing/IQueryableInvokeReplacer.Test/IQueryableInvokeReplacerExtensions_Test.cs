using IQuerableExtensions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;
using System.Data.Common;
using System.Linq;

namespace IQueryableInvokeReplacer.Test;

public class IQueryableInvokeReplacerExtensions_Test : BloggingTestBase
{
    [Fact]
    public void GetAllBlogs()
    {
        using var context = CreateContext();
        var a = context.Blogs.ToList();
        Assert.Collection(
            a,
            b => Assert.Equal("Blog1", b.Name),
            b => Assert.Equal("Blog2", b.Name));
    }

    [Fact]
    public void Dbcontext_Query()
    {
        var post = new Post { BlogId = 1, PostId = 2 };
        using var context = CreateContext();
        var a = context.Query(post).ToQueryString();
        post.PostId = 3;
        var b = context.Query(post).ToQueryString();
    }

    [Fact]
    public void Dbcontext_QueryMultiple_SigleKey()
    {
        Post[] posts = {
            new Post { BlogId = 1, PostId = 2 },
            new Post { BlogId = 2, PostId = 2 }
        };

        using var context = CreateContext();
        var a = context.Query<Post>(posts).ToList();
    }

    [Fact]
    public void Dbcontext_QueryMultiple_MultipleKey()
    {
        Comment[] comments = {
            new Comment { PostId = 1,index = 1 },
            new Comment { PostId = 1,index = 2 },
        };

        using var context = CreateContext();
        var a = context.Query<Comment>(comments).ToList();
    }
}
