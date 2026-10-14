using MudBlazor.Services;
using Anima.WebApp.Components;
using MudBlazor;
using Microsoft.AspNetCore.Components.Authorization;
using Anima.WebApp.Services.Authentication;
using ProtectedLocalStore;
using Blazored.LocalStorage;

var builder = WebApplication.CreateBuilder(args);

// Add MudBlazor services
builder.Services.AddMudServices(config => {
config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomRight; });

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// 1. Local Storage
builder.Services.AddLocalStorageServices();
builder.Services.AddBlazoredLocalStorage();

// 2. Custom Auth State Provider
// 1. The Middleman (Scoped is key!)
builder.Services.AddSingleton<UserTokenStore>();
builder.Services.AddScoped<CustomAuthStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(sp => sp.GetRequiredService<CustomAuthStateProvider>());
builder.Services.AddAuthorizationCore();

// 3. HTTP Client with the Authorization Handler
builder.Services.AddTransient<JwtAuthorizationHandler>();
builder.Services.AddHttpClient<AuthService>(client => client.BaseAddress = new Uri("http://localhost:5000/"))
    .AddHttpMessageHandler<JwtAuthorizationHandler>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);

app.UseHttpsRedirection();


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
