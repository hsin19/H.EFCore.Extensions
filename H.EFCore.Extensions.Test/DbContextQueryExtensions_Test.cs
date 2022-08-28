using H.EFCore.Extensions.Test.TestModel;
using H.EFCore.Extensions.Tools;

namespace H.EFCore.Extensions.Test;

public class DbContextQueryExtensions_Test : BloggingTestBase
{
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
            new Comment { PostId = 1, Index = 1 },
            new Comment { PostId = 1, Index = 2 },
        };

        using var context = CreateContext();
        var a = context.Query<Comment>(comments).ToList();
    }

    [Fact]
    public void LambdaConcatenation_t()
    {
        using var context = CreateContext();
        var a = context.Set<Post>()
            .Select(e => e.Title)
            .ToQueryString();
        var b = context.Set<Comment>()
            .Select(u => u.UserId, e => e.User)
            .ToQueryString();
    }

    [Fact]
    public void Type_t()
    {
        var ret1 = typeof(Comment).GetComplexProperty("PostId");
        var a = new { c = new Comment(), p = new Post() };
        var ret2 = a.GetType().GetComplexProperty("PostId");
        var ret3 = a.GetType().GetComplexProperty("BlogId");
        var b = new { u = new User(), cp = new { c = new Comment(), p = new Post() } };
        var ret4 = b.GetType().GetComplexProperty("PostId");
        var ret5 = b.GetType().GetComplexProperty("User.UserId");
        var ret6 = b.GetType().GetComplexProperty("c.User.UserId");
        var ret7 = b.GetType().GetComplexProperty("c.UserId");
        var ret8 = b.GetType().GetComplexProperty("User.Name");
        var ret9 = b.GetType().GetComplexProperty("c.Name");
    }

    [Fact]
    public void Order_t()
    {
        using var context = CreateContext();
        var a = context.Set<Post>().OrderBy("Title").ToQueryString();
        var b = context.Set<Comment>().OrderBy("User.UserId").ToQueryString();
        var c = context.Set<Comment>().OrderBy("PostId").OrderBy("User.UserId").ThenBy("PostId").ToQueryString();
        var d = context.Set<Comment>().OrderThenBy("PostId").OrderThenByDescending("User.UserId").OrderThenBy("Index").ToQueryString();
        var f = context.Set<Comment>().OrderBy("PostId,User.UserId Desc, Index ").ToQueryString();
    }
}
