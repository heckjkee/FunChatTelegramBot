using FunChatTelegramBot;
using Telegram.Bot;
using Telegram.Bot.Polling;

var builder = Host.CreateApplicationBuilder(args);
builder.Configuration.AddJsonFile("appsettings.json", false, true);
var botToken = builder.Configuration["TelegramBot:Token"]
    ?? throw new ArgumentException("Telegram bot token is missing from appsettings.json");
builder.Services.AddSingleton<ITelegramBotClient>(s => new TelegramBotClient(botToken));
builder.Services.AddSingleton<DbService>();
builder.Services.AddSingleton<TelegramHandler>();
builder.Services.AddSingleton<CommandHandler>();
builder.Services.AddSingleton<AppDbContext>();

builder.Services.AddHostedService<Worker>();
builder.Services.AddLogging();

var host = builder.Build();
await host.RunAsync();