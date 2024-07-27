using Microsoft.AspNetCore.Authentication;
using OrderManagement.Repositories;
using OrderManagement.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = "TwoFactorScheme";
    options.DefaultChallengeScheme = "TwoFactorScheme";
})
.AddScheme<AuthenticationSchemeOptions, TwoFactorAuthenticationHandler>("TwoFactorScheme", options => { });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireMFA", policy => policy.RequireAuthenticatedUser().RequireClaim("MFA"));
});

builder.Services.AddLogging();


builder.Services.AddDbContext<ApplicationDbContext>();
builder.Services.AddDbContext<CardholderDbContext>();



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.Use(async (context, next) =>
{
    var userId = context.User.Identity.IsAuthenticated ? context.User.Identity.Name : "Anonymous";
    var path = context.Request.Path;
    var method = context.Request.Method;
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    logger.LogInformation($"User: {userId}, Path: {path}, Method: {method}, Timestamp: {DateTime.UtcNow}");

    await next.Invoke();
});

app.UseHttpsRedirection(); // Ensure the use of TLS

// Middleware to ensure no data collection after authorization
app.Use(async (context, next) =>
{
    if (context.Request.Path.StartsWithSegments("/authorize"))
    {
        await next.Invoke();
    }
    else
    {
        // Ensure no cardholder data collection post-authorization
        if (context.Request.Path.StartsWithSegments("/collect"))
        {
            context.Request.Body = new MemoryStream();
            await context.Response.WriteAsync("No data collection post-authorization");
        }
        else
        {
            await next.Invoke();
        }
    }
});

app.MapGet("/", async context =>
{
    await context.Response.WriteAsync("Hello World!");
});

app.MapControllers();

app.Run();
