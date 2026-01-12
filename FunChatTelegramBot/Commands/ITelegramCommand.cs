using Telegram.Bot;
using Telegram.Bot.Types;

namespace FunChatTelegramBot.Commands;

public interface ITelegramCommand
{
    Task ExecuteAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken);
}