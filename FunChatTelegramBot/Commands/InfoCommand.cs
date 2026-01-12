using Telegram.Bot;
using Telegram.Bot.Types;

namespace FunChatTelegramBot.Commands;

[Command("/info", "Полная информация бота.")]
public class InfoCommand : ITelegramCommand
{
    public async Task ExecuteAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        var messageText = "Бот умеет выбирать пидора дня. Чтобы узнать команды используй /commands.";
        await botClient.SendMessage(update.Message.Chat.Id, messageText, disableNotification: true, cancellationToken: cancellationToken);
    }
}