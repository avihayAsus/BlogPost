using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using WebApplication2.DataBase;
using WebApplication2.Models;

namespace BlogPost.EndPoints
{
    public static class PostEndpoints
    {
        public static void MapPostsEndpoints(this IEndpointRouteBuilder routes)
        {
            var group = routes.MapGroup("/api/Post").WithTags(nameof(Post));

            group.MapGet("/", async (ApplicationDbContext db) => {
                return await db.Posts.ToListAsync();
            })
            .WithName("GetAllPosts")
            .WithOpenApi();

            group.MapGet("/{id}", async Task<Results<Ok<Post>, NotFound>> (Guid id, ApplicationDbContext db) => {
                return await db.Posts.Include(p => p.Comments).AsNoTracking()
                    .FirstOrDefaultAsync(model => model.Id == id)
                    is Post model
                        ? TypedResults.Ok(model)
                        : TypedResults.NotFound();
            })
            .WithName("GetPostById")
            .WithOpenApi();

            group.MapPut("/{id}", async Task<Results<Ok, NotFound>> (Guid id, Post post, ApplicationDbContext db) => {
                var affected = await db.Posts
                    .Where(model => model.Id == id)
                    .ExecuteUpdateAsync(setters => setters
                      .SetProperty(m => m.Id, post.Id)
                      .SetProperty(m => m.Title, post.Title)
                      .SetProperty(m => m.Description, post.Description)
                      .SetProperty(m => m.Author, post.Author)
                      .SetProperty(m => m.DateUpdated, DateTime.UtcNow)
                      );
                return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
            })
            .WithName("UpdatePost")
            .RequireAuthorization()
            .WithOpenApi();

            group.MapPost("/", async (Post post, ApplicationDbContext db) => {
                db.Posts.Add(post);
                await db.SaveChangesAsync();
                return TypedResults.Created($"/api/Post/{post.Id}", post);
            })
            .WithName("CreatePost")
            .RequireAuthorization()
            .WithOpenApi();

            group.MapDelete("/{id}", async Task<Results<Ok, NotFound>> (Guid id, ApplicationDbContext db) => {
                var affected = await db.Posts
                    .Where(model => model.Id == id)
                    .ExecuteDeleteAsync();
                return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
            })
            .WithName("DeletePost")
            .RequireAuthorization()
            .WithOpenApi();


            group.MapPost("/{id}/comment", async Task<Results<Ok, NotFound>> (Guid id, Comment comment, ApplicationDbContext db) => {
                var post = await db.Posts.Include(p => p.Comments).FirstOrDefaultAsync(x => x.Id == id);
                if (post is null) return TypedResults.NotFound();

                post.Comments.Add(comment);
                await db.SaveChangesAsync();

                return TypedResults.Ok();
            })
            .WithName("AddCommnent")
            .RequireAuthorization()
            .WithOpenApi();
        }
    }
}
