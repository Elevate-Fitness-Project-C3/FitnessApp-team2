using IdentityService.DataAccess.Context;
using IdentityService.DataAccess.Seeding;
using IdentityService.DependencyInjection;
using IdentityService.Features.Register.Endpoints;
using IdentityService.Messaging;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsDevelopment())
{
    builder.Configuration
        .AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: true)
        .AddEnvironmentVariables()
        .AddCommandLine(args);
}

builder.Services.AddOpenApi();
builder.Services.AddRegistrationMessaging(builder.Configuration);
builder.Services.AddIdentityService(builder.Configuration);

var app = builder.Build();

await using (var scope = app.Services.CreateAsyncScope())
{
    await IdentitySeeder.InitializeAsync(
        scope.ServiceProvider.GetRequiredService<ApplicationDbContext>(),
        scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>(),
        applyMigrations: true);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "Identity Service v1");
    });
}


app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapRegisterEndpoint();




app.Run();
