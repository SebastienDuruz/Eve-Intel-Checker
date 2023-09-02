using ElectronNET.API;
using EveIntelCheckerLib.Data;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddMudServices();
builder.Services.AddSingleton(new CustomSoundPlayer("notif_1.wav", "danger_1.wav", "notif_2.wav", "danger_2.wav"));

builder.WebHost.UseElectron(args);
builder.WebHost.UseUrls("http://localhost:31696");

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

// Support Electron
if (HybridSupport.IsElectronActive)
    await ElectronHandler.CreateElectronWindow();

app.Run();
