using System.Text.Json;
using System.Windows.Input;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace FunChatTelegramBot.Commands;

[Command("/run", "Основная команда бота. Ищет пидора дня.")]
public class RunCommand : ITelegramCommand
{
    private readonly DbService _service = new();
    public async Task ExecuteAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        var isChatExist = await _service.IsChatExistAsync(update, cancellationToken);
        if (!isChatExist)
            return;
        if (await _service.CanRunTodayAsync(update, cancellationToken))
        {
            var userOfDay = await _service.IncrementCounterAsync(update, cancellationToken);
            var phrases = await GetTextMessagesAsync();
            foreach (var phrase in phrases)
            {
                await botClient.SendMessage(update.Message.Chat.Id, phrase, cancellationToken: cancellationToken);
                await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
            }
            await botClient.SendMessage(update.Message.Chat.Id,
                $"Пидор дня: {userOfDay.User.FirstName} {userOfDay.User.LastName}",
                                    cancellationToken: cancellationToken);
            await _service.SetRunDateAsync(update, cancellationToken);
        }
        else
        {
            await botClient.SendMessage(update.Message.Chat.Id,
                "Сегодня команда уже использовалась", cancellationToken: cancellationToken);
        }
    }

    private async Task<List<string>> GetTextMessagesAsync()
    {
        var asm = typeof(RunCommand).Assembly;
        await using var stream = asm.GetManifestResourceStream("FunChatTelegramBot.Phrases.json");
        if (stream == null) return new List<string>();
        return await JsonSerializer.DeserializeAsync<List<string>>(stream)
            ?? new List<string>();
    }
}