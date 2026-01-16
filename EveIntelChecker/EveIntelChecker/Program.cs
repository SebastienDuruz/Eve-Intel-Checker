using ElectronNET;
using ElectronNET.API;
using EveIntelCheckerLib.Data;
using EveIntelCheckerLib.Services;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(new WebApplicationOptions());

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddMudServices();
builder.Services.AddElectron();
builder.Services.AddSingleton(new CustomSoundPlayer("notif_1.wav", "danger_1.wav", "notif_2.wav", "danger_2.wav"));
builder.Services.AddScoped<ILogFileReader, LogFileReader>();
builder.Services.AddSingleton<IIntelMessageProcessor, IntelMessageProcessor>();
builder.Services.AddSingleton<IMapDataBuilder, MapDataBuilder>();
builder.UseElectron(args, ElectronAppReady);

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

app.Run();

static async Task ElectronAppReady()
{
    if (!ElectronHandler.SetupSettings())
    {
        LogsWriter.Instance.Log(StaticData.LogLevel.Error, "Failed to setup the settings. The application will be closed.");
        Electron.App.Exit();
        return;
    }

    await ElectronHandler.CreateElectronWindow();
}
