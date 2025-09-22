using Blazored.LocalStorage;
using LibrAI.Client;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddMudServices();
// Hardcode yerine host origin'i kullan:
builder.Services.AddScoped(sp =>
    new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddScoped<AuthClient>();
builder.Services.AddScoped<AuthenticationStateProvider, TokenAuthStateProvider>();
builder.Services.AddAuthorizationCore();

var host = builder.Build();

// Uygulama açılırken token'ı header'a geri yükle
await host.Services.GetRequiredService<AuthClient>().RestoreAsync();

// Çalıştır
await host.RunAsync();
