using System.Reflection;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace FunChatTelegramBot.Commands;

[Command("/commands", "Список команд")]
public class CommandsCommand : ITelegramCommand
{
    public async Task ExecuteAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        var messageText = "";
        var assembly = Assembly.GetExecutingAssembly();
        var commandTypes = assembly.GetTypes()
            .Where(t => t.GetCustomAttribute<CommandAttribute>() != null)
            .ToList();
        foreach (var command in commandTypes)
        {
            var attribute = command.GetCustomAttribute<CommandAttribute>();
            if (attribute != null)
                messageText += $"{attribute.Command} - {attribute.Description}\n";
        }
        await botClient.SendMessage(update.Message.Chat.Id, messageText, disableNotification: true, cancellationToken: cancellationToken);
    }
}