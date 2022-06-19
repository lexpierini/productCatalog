using backendMinimalApi.Models;
using backendMinimalApi.Services;
using Microsoft.AspNetCore.Authorization;

namespace backendMinimalApi.Endpoints
{
    public static class Authentication
    {
        // Extension method
        public static void MapAuthenticationEP(this WebApplication app)
        {
            app.MapPost("/login", [AllowAnonymous] (UserModel userModel, ITokenService tokenService) =>
            {
                if (userModel == null) return Results.BadRequest("Invalid Login");

                if (userModel.UserName == "admin" && userModel.Password == "admin#123")
                {
                    var tokenString = tokenService.MakeToken(app.Configuration["Jwt:Key"],
                        app.Configuration["Jwt:Issuer"],
                        app.Configuration["Jwt:Audience"],
                        userModel);

                    return Results.Ok(new { token = tokenString });
                }
                else
                {
                    return Results.BadRequest("Invalid Login");
                }
            }).Produces(StatusCodes.Status400BadRequest)
              .Produces(StatusCodes.Status200OK)
              .WithName("Login")
              .WithTags("Authentication");
        }
    }
}
