using System;
using System.Runtime.InteropServices;
using ElectronNET.API;
using EveIntelCheckerLib.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddMudServices();
builder.Services.AddSingleton(new CustomSoundPlayer("notif_1.wav", "danger_1.wav", "notif_2.wav", "danger_2.wav"));

builder.WebHost.UseElectron(args);
builder.WebHost.UseUrls($"http://localhost:{StaticData.ApplicationPort}");

var app = builder.Build();

 // Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseStaticFiles();
app.UseRouting();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

// Set a limit to MALLOC_TRIM (reduce RAM usage on Linux)
Environment.SetEnvironmentVariable("MALLOC_TRIM_THRESHOLD_", "100000");

// Support Electron
if (HybridSupport.IsElectronActive)
    if (await ElectronHandler.SetupSettings())
        await ElectronHandler.CreateElectronWindow();
    else
        Electron.App.Exit();

app.Run();
