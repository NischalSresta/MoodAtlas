using Microsoft.Extensions.Logging;
using MoodAtlas.Config;
using MoodAtlas.Services;

namespace MoodAtlas;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
			});

		builder.Services.AddMauiBlazorWebView();

		builder.Services.AddSingleton<DatabaseConfig>();
		
		// Register services
		builder.Services.AddSingleton<UserService>();
		builder.Services.AddSingleton<EntryService>();
        builder.Services.AddSingleton<MoodService>();
        builder.Services.AddSingleton<TagService>();
        builder.Services.AddSingleton<CategoryService>();
        builder.Services.AddSingleton<AnalyticsService>();
        builder.Services.AddSingleton<ExportService>();

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
		builder.Logging.AddDebug();
#endif
		
		return builder.Build();
	}
}
