using BlogPost.Services;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using WebApplication2.Models;

namespace BlogPost.EndPoints
{
    public static class AuthEndPoints
    {
        public static void MapAuthEndpoints(this IEndpointRouteBuilder routes)
        {
            var group = routes.MapGroup("/api/Auth").WithTags("Auth");

            group.MapPost("/token", async Task<Results<Ok<AccessTokenResponse>, Ok<string>, ProblemHttpResult, UnauthorizedHttpResult>>
                ([FromBody] LoginRequest login, SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, IUserService userService) => {

                    var user = await userManager.FindByNameAsync(login.Email);
                    if (user is null) return TypedResults.Unauthorized();

                    var result = await userManager.CheckPasswordAsync(user, login.Password);

                    if (!result)
                    {
                        return TypedResults.Problem(result.ToString(), statusCode: StatusCodes.Status401Unauthorized);
                    }

                    return TypedResults.Ok(userService.CreateToken(user));
                })
            .WithName("token")
            .WithOpenApi();

        }
    }
}
