namespace FunChatTelegramBot.Commands;

[AttributeUsage(AttributeTargets.Class)]
public class CommandAttribute : Attribute
{
    public string Command { get; set; }
    public string Description { get; set; }

    public CommandAttribute(string command, string description)
    {
        Command = command;
        Description = description;
    }
}