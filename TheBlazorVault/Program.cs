using TheBlazorVault.Components;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using MudBlazor.Services;
using TheBlazorVault.Service;

var builder = WebApplication.CreateBuilder(args);
// Ajouter l'authentification JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(builder.Configuration, "AzureAd");


// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

//Lecture des informations de la WebAPI depuis le fichier de configuration.
string apiEndpoint = builder.Configuration.GetValue<string>("WebAPI:Endpoint") ?? throw new InvalidOperationException("WebAPI is not configured");
string apiScope = builder.Configuration.GetValue<string>("WebAPI:Scope") ?? throw new InvalidOperationException("WebAPI is not configured");

builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApp(builder.Configuration, "AzureAd")
    .EnableTokenAcquisitionToCallDownstreamApi([apiScope])
    .AddDownstreamApi("EntraIDAuthWebAPI", options =>
    {
        options.BaseUrl = apiEndpoint;
        options.Scopes = [apiScope];
    })
    .AddInMemoryTokenCaches();


builder.Services
    .AddControllersWithViews()
    .AddMicrosoftIdentityUI();



builder.Services.AddAuthorization();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddMudServices();

builder.Services.AddScoped<CallServices>();
builder.Services.AddScoped<CryptoHelper>();

var app = builder.Build();

// … vos checks d’environnement, HSTS, HTTPS …

app.UseRouting();

// Active le pipeline d’authentification / autorisation
app.UseAuthentication();
app.UseAuthorization();

// Expose les controllers de MicrosoftIdentityUI (/MicrosoftIdentity/Account/…)
app.MapControllers();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();

// // Configure the HTTP request pipeline.
// if (!app.Environment.IsDevelopment())
// {
//     app.UseExceptionHandler("/Error", createScopeForErrors: true);
//     // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
//     app.UseHsts();
// }
// 
// 
// app.UseHttpsRedirection();
// 
// app.UseAntiforgery();
// 
// app.MapStaticAssets();
// app.MapRazorComponents<App>()
//     .AddInteractiveServerRenderMode();
// 
// app.Run();