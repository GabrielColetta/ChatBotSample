using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SampleAI.IoC.Extensions;
using SampleAI.Shared.Configurations;

namespace SampleAI.App;

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

        builder.Configuration.AddJsonFile("appsettings.json");

        builder.Services.AddMauiBlazorWebView();
		builder.Services.AddBlazorBootstrap();
		builder.Services.AddDefaultAppServices(builder.Configuration);
        builder.Services.AddOptions<SocketConfiguration>()
            .Bind(builder.Configuration.GetSection("websocket"));
#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
