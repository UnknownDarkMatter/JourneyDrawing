using JourneyDrawing.Front.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.DependencyInjection;



var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers(config =>
{
    config.Filters.Add<GlobalLogActionFilter>();
    config.OutputFormatters.RemoveType<HttpNoContentOutputFormatter>();
});


builder.Services.AddRazorPages((options) =>
{
    options.Conventions.AddPageRoute("/Index", "{*url}");
});


builder.Services.AddCors((options) =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy
            .WithOrigins(builder.Configuration.GetValue<string>("CorsAllowedOrigin").Split(';'))
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});


var app = builder.Build();

app.UseCors();

app.UseStaticFiles();

app.UseRouting();

//app.UseAuthentication();
//app.UseAuthorization();
app.MapControllers();
app.MapRazorPages();


app.Run();
