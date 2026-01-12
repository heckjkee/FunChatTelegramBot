using Telegram.Bot;
using Telegram.Bot.Types;

namespace FunChatTelegramBot;

public static class CommandHandler
{
    public static async Task HandleCommandAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
        //TODO: реализована работа с базой данных, реализовано добавление юзеров и чатов. Теперь надо реализовать команды.
    }
}