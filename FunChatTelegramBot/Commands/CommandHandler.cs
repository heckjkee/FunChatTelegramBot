using System.Reflection;
using System.Windows.Input;
using FunChatTelegramBot.Commands;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace FunChatTelegramBot;

public class CommandHandler
{
    private readonly Dictionary<string, Type> _commands;

    public CommandHandler()
    {
        _commands = CreateCommands();
    }
    
    public async Task HandleCommandAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        if (_commands.TryGetValue(update.Message.Text.Split('@')[0], out var commandType))
        {
            var command = Activator.CreateInstance(commandType) as ITelegramCommand;
            await command.ExecuteAsync(botClient, update, cancellationToken);
        }
    }

    private Dictionary<string, Type> CreateCommands()
    {
        var commandMap = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(t => t.GetInterfaces().Contains(typeof(ITelegramCommand)) && 
                        t.GetCustomAttribute<CommandAttribute>() != null)
            .ToDictionary(
                t => t.GetCustomAttribute<CommandAttribute>().Command,
                t => t);
        return commandMap;
    }
}