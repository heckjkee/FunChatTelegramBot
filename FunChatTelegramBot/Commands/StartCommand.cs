using Telegram.Bot;
using Telegram.Bot.Types;

namespace FunChatTelegramBot.Commands;

[Command("/start", "Приветственная команда для бота и регистрация")]
public class StartCommand : ITelegramCommand
{
    private readonly DbService _dbService = new ();
    public async Task ExecuteAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        var messageText = "Приветствую. Это телеграм-бот для чатов. Первая функция - выбор пидора дня. Далее будет больше" +
                          "функционала. Введите /info чтобы узнать полное описание.";
        await _dbService.AddChatAsync(update, cancellationToken);
        await botClient.SendMessage(update.Message.Chat.Id, messageText, disableNotification: true, cancellationToken: cancellationToken);
    }
}