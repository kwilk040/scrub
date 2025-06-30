using Application.Services;
using Application.Services.Interfaces;
using Core.Model;
using Infrastructure;
using Microsoft.AspNetCore.Identity;
using ParserService;
using WebUI.Components;
using WebUI.Components.Features.UserAside;
using WebUI.Services;
using WebUI.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Identity
builder.Services.AddCascadingAuthenticationState();

// Infrastructure
builder.Services.AddTransientInfrastructure(builder.Configuration);
builder.Services.AddAuth();

// Application
builder.Services.AddScoped<IScrobbleService, ScrobbleService>();
builder.Services.AddScoped<IUserSessionService, UserSessionService>();
builder.Services.AddScoped<UserAsideService>();
builder.Services.AddScoped<IApiKeyService, ApiKeyService>();

// UI
builder.Services.AddScoped<IUserSessionInitializer, UserSessionInitializer>();
builder.Services.AddScoped<IUiNotificationService, UiNotificationService>();
builder.Services.AddScoped<IUiHelperService, UiHelperService>();
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/login";
    options.LogoutPath = "/logout";
});
builder.Services.AddHttpContextAccessor();
builder.Services
    .AddGrpcClient<Parser.ParserClient>(o => { o.Address = new Uri("http://localhost:5140"); });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await SeedRolesAndAdminAsync(services);
}

app.MapAdditionalIdentityEndpoints();
app.Run();


async Task SeedRolesAndAdminAsync(IServiceProvider services)
{
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

    if (!await roleManager.RoleExistsAsync("Admin"))
    {
        await roleManager.CreateAsync(new IdentityRole("Admin"));
    }

    var adminUser = await userManager.FindByNameAsync("admin");

    if (adminUser != null && !await userManager.IsInRoleAsync(adminUser, "Admin"))
    {
        await userManager.AddToRoleAsync(adminUser, "Admin");
    }
}