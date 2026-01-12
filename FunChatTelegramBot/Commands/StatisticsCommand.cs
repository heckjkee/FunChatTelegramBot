using System.Text;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace FunChatTelegramBot.Commands;

[Command("/stats", "Получает статистику по пидорам дня.")]
public class StatisticsCommand : ITelegramCommand
{
    private readonly DbService _service = new ();
    public async Task ExecuteAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        var userCounters = await _service.GetStatisticsAsync(update, cancellationToken);
        var sb = new StringBuilder();
        var counter = 1;
        sb.Append("Пидоры дня:\n");
        foreach (var userCounter in userCounters)
        {
            sb.Append($"{counter}. {userCounter.User.FirstName} {userCounter.User.LastName} - {userCounter.Counter}\n");
            counter++;
        }
        await botClient.SendMessage(update.Message.Chat.Id,sb.ToString(), cancellationToken: cancellationToken);
    }
}