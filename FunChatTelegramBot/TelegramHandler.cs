using System.Net.Mime;
using System.Threading.Channels;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace FunChatTelegramBot;

public class TelegramHandler : IUpdateHandler
{
    private readonly DbService _dbService;
    private readonly ILogger<TelegramHandler> _logger;
    private readonly CommandHandler _commands;

    public TelegramHandler(DbService dbService, ILogger<TelegramHandler> logger, CommandHandler commands)
    {
        _dbService = dbService;
        _logger = logger;
        _commands = commands;
    }
    
    public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        if (update.Message.Chat.Type == ChatType.Private)
        {
            await botClient.SendMessage(update.Message.Chat.Id, "Бот не предназначен для использования" +
                                                                " в личных сообщениях");
            return;
        }
        var updateHandleTask = update.Message.Type switch
        {
            MessageType.Text => HandleTextMessage(botClient, update, cancellationToken),
            _ => HandleOtherMessage(botClient, update, cancellationToken)
        };
        await updateHandleTask;
    }

    private async Task HandleTextMessage(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"User {update.Message.From.Id} wrote {update.Message.Text}");
        await UpdateDataAsync(update, cancellationToken);
        var message = update.Message.Text.Split('@')[0];
        if (message.StartsWith("/"))
            await _commands.HandleCommandAsync(botClient, update, cancellationToken);
    }
    private async Task HandleOtherMessage(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"User {update.Message.From.Id} send other message");
        await UpdateDataAsync(update, cancellationToken);
    }


    public Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, HandleErrorSource source,
        CancellationToken cancellationToken)
    {
        var loggingException = $"Telegram global exception at {DateTime.Now}: {exception.Message} {exception.StackTrace}";
        _logger.LogError(loggingException);
        return Task.CompletedTask;
    }

    private async Task UpdateDataAsync(Update update, CancellationToken cancellationToken)
    {
        var isChatExists = await _dbService.IsChatExistAsync(update, cancellationToken);
        var isUserExists = await _dbService.IsUserExistAsync(update, cancellationToken);
        if (!isChatExists)
        {
            await _dbService.AddChatAsync(update, cancellationToken);
            _logger.LogInformation($"Chat {update.Message.Chat.Id} has been created");
        }

        if (!isUserExists)
        {
            await _dbService.AddUserAsync(update, cancellationToken);
            _logger.LogInformation($"User {update.Message.Chat.Id} has been created");
        }
        var isCounterExists = await _dbService.IsCounterExistAsync(update, cancellationToken);
        if (!isCounterExists)
        {
            await _dbService.InitalizeUserCounterAsync(update, cancellationToken);
            _logger.LogInformation($"Counter {update.Message.Chat.Id} has been initalized");
        }
        
    }
}