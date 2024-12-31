using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SurpassIntegration.Data;
using SurpassIntegration.Models;
using SurpassIntegration.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// 1. Configure EF Core with a connection string
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("AppDbContext")));

// 2. Add JWT Authentication
var secretKey = builder.Configuration["JwtSettings:SecretKey"];
builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false; // For dev only
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!))
        };
    });

// 3. Add Controllers
builder.Services.AddControllers();

builder.Services.AddScoped<IUserService, UserService>();


var app = builder.Build();

// 4. Use Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// 5. Migrate DB at startup (optional, but convenient for dev)
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.Migrate();

    if (!dbContext.Users.Any())
{
    dbContext.Users.Add(new User
    {
        Username = "johndoe",
        Password = "Pass123" // In real scenarios, store a hashed password
    });
    dbContext.SaveChanges();
}
}

app.Run();
