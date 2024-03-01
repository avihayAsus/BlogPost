using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using WebApplication2.DataBase;
using WebApplication2.Models;

namespace BlogPost.EndPoints
{
    public static class CommentEndpoints
    {
        public static void MapCommentEndpoints(this IEndpointRouteBuilder routes)
        {
            var group = routes.MapGroup("/api/Comment").WithTags(nameof(Comment));

            group.MapGet("/", async (ApplicationDbContext db) => {
                return await db.Comments.ToListAsync();
            })
            .WithName("GetAllComments")
            .WithOpenApi();

            group.MapGet("/{id}", async Task<Results<Ok<Comment>, NotFound>> (Guid id, ApplicationDbContext db) => {
                return await db.Comments.AsNoTracking()
                    .FirstOrDefaultAsync(model => model.Id == id)
                    is Comment model
                        ? TypedResults.Ok(model)
                        : TypedResults.NotFound();
            })
            .WithName("GetCommentById")
            .WithOpenApi();


            group.MapPut("/{id}", async Task<Results<Ok, NotFound>> (Guid id, Comment comment, ApplicationDbContext db) => {
                var affected = await db.Comments
                    .Where(model => model.Id == id)
                    .ExecuteUpdateAsync(setters => setters
                      .SetProperty(m => m.Id, comment.Id)
                      .SetProperty(m => m.Description, comment.Description)
                      .SetProperty(m => m.Date, DateTime.Now)
                      .SetProperty(m => m.PostId, comment.PostId)
                      );
                return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
            })
            .WithName("UpdateComment")
            .RequireAuthorization()
            .WithOpenApi();

            group.MapPost("/", async Task<Results<BadRequest, Created<Comment>>> (Comment comment, ApplicationDbContext db) => {

                var post = db.Posts.SingleOrDefault(p => p.Id == comment.Id);
                if (post is null) return TypedResults.BadRequest();

                db.Comments.Add(comment);
                await db.SaveChangesAsync();
                return TypedResults.Created($"/api/Comment/{comment.Id}", comment);
            })
            .WithName("CreateComment")
            .RequireAuthorization()
            .WithOpenApi();

            group.MapDelete("/{id}", async Task<Results<Ok, NotFound>> (Guid id, ApplicationDbContext db) => {
                var affected = await db.Comments
                    .Where(model => model.Id == id)
                    .ExecuteDeleteAsync();
                return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
            })
            .WithName("DeleteComment")
            .RequireAuthorization()
            .WithOpenApi();
        }
    }
}
