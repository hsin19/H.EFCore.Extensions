namespace H.EFCore.Extensions.Test;

public class DbContextQueryExtensions_Test : BloggingTestBase
{
    [Fact]
    public void Dbcontext_Query()
    {
        var post = new Post { BlogId = 1, PostId = 2 };
        using var context = CreateContext();
        string? a = context.Query(post).ToQueryString();
        post.PostId = 3;
        string? b = context.Query(post).ToQueryString();
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
}
