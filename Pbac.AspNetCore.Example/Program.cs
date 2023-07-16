using System.Security.Claims;
using System.Text;
using Pbac.AspNetCore;
using Pbac.AspNetCore.Example;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

// Toggle between the Minimal APIs and Controllers
const bool UseMinimalApis = true;

var builder = WebApplication.CreateBuilder(args);

if (UseMinimalApis)
    builder.Services.AddEndpointsApiExplorer();
else
    builder.Services.AddControllers();

builder.Services.AddAuthentication().AddJwtBearer(options =>
{
    if (builder.Environment.IsDevelopment())
    {
        options.TokenValidationParameters.IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(DummyJwtGenerator.JwtSecret));

        options.TokenValidationParameters.ValidateAudience = false;
        options.TokenValidationParameters.ValidateIssuer = false;
        options.TokenValidationParameters.RequireExpirationTime = false;
    }
});
builder.Services.AddAuthorization();
builder.Services.AddPermissionBasedAuthorization<Permissions>(options =>
    options.PermissionsClaimName = ClaimNames.Permissions);

var app = builder.Build();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

if (UseMinimalApis)
{
    app.MapGet("/jwt", ([FromQuery] Permissions[] permissions) =>
    {
        var permissionString = new PermissionSet<Permissions>(permissions).ToCompactString();
        return DummyJwtGenerator.Generate(new[] { new Claim(ClaimNames.Permissions, permissionString) });
    });

    app.MapGet("/entity", () => Results.NoContent())
    .RequirePermission(Permissions.ReadEntity);

    app.MapDelete("/entity", () => Results.NoContent())
    .RequirePermission(Permissions.DeleteEntity);
}
else
{
    app.MapControllers();
}

app.Run();
