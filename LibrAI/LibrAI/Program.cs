using Microsoft.EntityFrameworkCore;
using LibrAI.Data;
using LibrAI.Data.Repositories;
using LibrAI.Services;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<BookRepository>();
builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<BookService>();

// Veritabaný baðlantýsýný burada yapýlandýrýyoruz
builder.Services.AddDbContext<LibrAIDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("LibrAIDbConnection")));

builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<LibrAIDbContext>()
    .AddDefaultTokenProviders();

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Veritabaný baþlangýç iþlemleri
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
    await SeedRoles.Initialize(services, userManager);  // SeedRoles'u çaðýrýyoruz
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
