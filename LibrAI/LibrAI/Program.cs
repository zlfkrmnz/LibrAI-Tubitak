using Blazored.LocalStorage;
using LibrAI.Client;
using LibrAI.Data;
using LibrAI.Data.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

// Prerender/SSR sırasında doğru origin'e gidebilmek için:
builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient();

builder.Services.AddScoped(sp =>
{
    var factory = sp.GetRequiredService<IHttpClientFactory>();
    var httpContext = sp.GetRequiredService<IHttpContextAccessor>().HttpContext;

    // Mevcut isteğin Scheme + Host bilgisi (ör: https://localhost:7150/)
    var baseUri = httpContext is not null
        ? $"{httpContext.Request.Scheme}://{httpContext.Request.Host.Value}/"
        : "https://localhost:7150/"; // fallback: launchSettings'teki https portun

    var client = factory.CreateClient();
    client.BaseAddress = new Uri(baseUri);
    return client;
});

// UI & API
builder.Services.AddMudServices();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// DbContext
builder.Services.AddDbContext<LibrAiDbContext>(opt =>
    opt.UseSqlite(builder.Configuration.GetConnectionString("LibraiDb")));

// CORS – yalnızca farklı origin'den çağıracaksan gerek
builder.Services.AddCors(opt =>
{
    opt.AddPolicy("client", p => p
        .AllowAnyHeader()
        .AllowAnyMethod()
        // Client'ı ayrı porttan çalıştıracaksan buraya **client originini** yaz
        .WithOrigins("https://localhost:7150", "http://localhost:5069")
        .AllowCredentials());
});

// Blazor
builder.Services.AddRazorComponents()
    .AddInteractiveWebAssemblyComponents();

// DI
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddScoped<UserSessionService>();
builder.Services.AddScoped<IBookRepository, BookRepository>();
builder.Services.AddScoped<IPublisherRepository, PublisherRepository>();
builder.Services.AddScoped<IBookService, BookService>();

builder.Services.AddDbContext<AuthDbContext>(opt =>
    opt.UseSqlite(builder.Configuration.GetConnectionString("LibraiDb")));

builder.Services.AddIdentityCore<AppUser>(opt =>
{
    opt.User.RequireUniqueEmail = true;
    opt.Password.RequireNonAlphanumeric = false;
    opt.Password.RequireUppercase = false;
    opt.Password.RequiredLength = 6;
})
    .AddEntityFrameworkStores<AuthDbContext>()
    .AddApiEndpoints();                       // <-- Identity API endpoints

builder.Services.AddAuthentication()
    .AddBearerToken(IdentityConstants.BearerScheme); // <-- token auth

builder.Services.AddAuthorization();


var app = builder.Build();

// middleware
app.UseAuthentication();
app.UseAuthorization();

// Identity minimal API’leri:
app.MapIdentityApi<AppUser>();


if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// Eğer client ayrı origin'de koşuyorsa CORS'u aktif et:
app.UseCors("client");

app.UseAntiforgery();

app.MapControllers();
app.MapStaticAssets();
app.MapRazorComponents<App>()
   .AddInteractiveWebAssemblyRenderMode();

app.Run();
