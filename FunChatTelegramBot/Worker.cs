using Microsoft.EntityFrameworkCore;
using Telegram.Bot;

namespace FunChatTelegramBot;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly ITelegramBotClient _botClient;
    private readonly TelegramHandler _handler;
    private readonly CancellationTokenSource _cancellationTokenSource;
    private readonly IServiceProvider _serviceProvider;

    public Worker(ILogger<Worker> logger, ITelegramBotClient botClient, TelegramHandler handler, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _botClient = botClient;
        _handler = handler;
        _cancellationTokenSource = new CancellationTokenSource();
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation($"{DateTime.Now} TelegramBotService is starting.");
        using var scope = _serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var dbExists = await db.Database.CanConnectAsync(stoppingToken);
        if (!dbExists)
        {
            await db.Database.MigrateAsync(stoppingToken);
        }
        try
        {
            var me = await _botClient.GetMe(stoppingToken);
            _logger.LogInformation($"{DateTime.Now} Bot {me.Username} has been started.");
            _botClient.StartReceiving(_handler, cancellationToken: stoppingToken);
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"{DateTime.Now} Ошибка.");
            Console.WriteLine(e);
        }
    }
}