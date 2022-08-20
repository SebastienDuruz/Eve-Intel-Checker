using ElectronNET.API;
using ElectronNET.API.Entities;
using EveIntelChecker.Data;
using EveIntelChecker.ElectronApp;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Configuration;
using MudBlazor.Services;
using System.Configuration;

var builder = WebApplication.CreateBuilder(args);

try
{
    // Add services to the container.
    builder.Services.AddRazorPages();
    builder.Services.AddServerSideBlazor();
    builder.Services.AddMudServices();
    builder.Services.AddElectron();
    builder.Services.AddSingleton<EVE_API>();

    builder.WebHost.UseElectron(args);

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

    if (HybridSupport.IsElectronActive)
    {
        await ElectronHandler.CreateElectronWindow();
    }

    app.Run();
}
catch(Exception ex)
{

}
