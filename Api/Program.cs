using EntityFrameworkComm.EfModel.Context;
using Microsoft.Net.Http.Headers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;
using Microsoft.OpenApi.Models;
using Api.Middleware;
using Api.Repositories;
using Api.Service;

var builder = WebApplication.CreateBuilder(args);

string? corsFrontEndpoint = builder.Configuration.GetValue<string>("CorsFrontEndpoint");

//CORS permet d'autoriser le front � appeler l'API.
if (string.IsNullOrWhiteSpace(corsFrontEndpoint) == false)
{
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("WebAssemblyOrigin", policy =>
        { 
            policy
                .WithOrigins(corsFrontEndpoint)
                .AllowAnyMethod()
                .WithHeaders(HeaderNames.ContentType, HeaderNames.Authorization, "x-custom-header")
                .AllowCredentials();
        });
    });
}

builder.Services.AddControllers();

//builder.Services.AddControllers().AddNewtonsoftJson();

// Add services to the container.
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));

//Middlewares
builder.Services.AddScoped<GetOrCreateAppUserIdMiddleware>();

builder.Services.AddDbContextFactory<Context>(options => 
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<UserRepositorie>();

builder.Services.AddScoped<UserService>();


// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
 builder.Services.AddOpenApi();

 builder.Services.AddSwaggerGen(c =>
 {
     c.SwaggerDoc("v1", new OpenApiInfo { Title = "API", Version = "v1" });
 });





 var app = builder.Build();

// Configure the HTTP request pipeline.
 if (app.Environment.IsDevelopment())
 {
     app.UseDeveloperExceptionPage();
     app.UseSwagger();
     app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Api v1"));
 }

// Ensure the database is created
 using (var scope = app.Services.CreateScope())
 {
     var dbContext = scope.ServiceProvider.GetRequiredService<Context>();
     dbContext.Database.EnsureCreated();
 }

 app.UseHttpsRedirection();

 if (string.IsNullOrWhiteSpace(corsFrontEndpoint) == false)
 {
     app.UseCors("WebAssemblyOrigin");
 }

 app.UseAuthentication();

 app.UseAuthorization();

 app.UseMiddleware<GetOrCreateAppUserIdMiddleware>();

 app.MapControllers().RequireAuthorization();

 app.Run();