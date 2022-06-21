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
        var asda = context.Blogs.ToQueryString();
        Assert.Collection(
            a,
            b => Assert.Equal("Blog1", b.Name),
            b => Assert.Equal("Blog2", b.Name));
    }
}
