using ElectronNET.API;
using EveIntelCheckerElectron.Data;
using EveIntelCheckerLib.Data;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

UserSettingsReader userSettings = new UserSettingsReader();

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSingleton(new EveStaticDatabase(HybridSupport.IsElectronActive));
builder.Services.AddSingleton(userSettings);
builder.Services.AddMudServices();

builder.WebHost.UseElectron(args);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

// Support Electron
if (HybridSupport.IsElectronActive)
    await ElectronHandler.CreateElectronWindow((int)userSettings.UserSettingsValues.WindowWidth, (int)userSettings.UserSettingsValues.WindowHeight);

app.Run();
