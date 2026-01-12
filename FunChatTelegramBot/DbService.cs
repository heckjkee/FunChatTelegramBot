using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot.Types;

namespace FunChatTelegramBot;

public class DbService
{
    private readonly AppDbContext _dbContext = new();
    
    public async Task<bool> IsChatExistAsync(Update update, CancellationToken cancellationToken) 
        => await _dbContext.Chats
            .AnyAsync(c => c.ChatId == update.Message.Chat.Id, cancellationToken);
    public async Task<bool> IsUserExistAsync(Update update, CancellationToken cancellationToken)
        => await _dbContext.Users
            .AnyAsync(c => c.UserId == update.Message.From.Id, cancellationToken);
    public async Task<bool> IsCounterExistAsync(Update update, CancellationToken cancellationToken)
        => await _dbContext.Counters
            .AnyAsync(c => c.UserId == update.Message.From.Id && c.ChatId == update.Message.Chat.Id, cancellationToken);

    public async Task AddChatAsync(Update update, CancellationToken cancellationToken)
    {
        if (!await IsChatExistAsync(update, cancellationToken))
        {
            var chat = new Chat()
            {
                ChatId = update.Message.Chat.Id,
                UserCounters = new List<UserChatCounter>()
            };
            _dbContext.Chats.Add(chat);
            await _dbContext.SaveChangesAsync(cancellationToken);
            await InitializeRunDateAsync(update, cancellationToken);
        }
    }

    public async Task AddUserAsync(Update update, CancellationToken cancellationToken)
    {
        if (!await IsUserExistAsync(update, cancellationToken))
        {
            var user = new User()
            {
                UserId = update.Message.From.Id,
                FirstName = update.Message.From.FirstName,
                LastName = update.Message.From.LastName,
                ChatCounters = new List<UserChatCounter>()
            };
            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task InitalizeUserCounterAsync(Update update, CancellationToken cancellationToken)
    {
        if (await IsUserExistAsync(update, cancellationToken) && await IsChatExistAsync(update, cancellationToken))
        {
            var chat = await _dbContext.Chats
                .Include(c => c.UserCounters)
                .FirstAsync(c => c.ChatId == update.Message.Chat.Id, cancellationToken);
            var user = await _dbContext.Users
                .FirstAsync(u => u.UserId == update.Message.From.Id, cancellationToken);
            var counter = new UserChatCounter()
            {
                UserId = user.UserId,
                ChatId = chat.ChatId,
                Chat = chat,
                User = user,
                Counter = 0
            };
            chat.UserCounters.Add(counter);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<UserChatCounter> GetRandomUserCounter(Update update, CancellationToken cancellationToken)
    {
        var chat = await  _dbContext.Chats
            .Include(c => c.UserCounters)
            .ThenInclude(c => c.User)
            .FirstAsync(c => c.ChatId == update.Message.Chat.Id, cancellationToken);
        var index = RandomNumberGenerator.GetInt32(0, chat.UserCounters.Count);
        return chat.UserCounters.ElementAt(index); 
    }

    public async Task<UserChatCounter> IncrementCounterAsync(Update update, CancellationToken cancellationToken)
    {
        var counter = await GetRandomUserCounter(update, cancellationToken);
        counter.Counter++;
        await _dbContext.SaveChangesAsync(cancellationToken);
        return counter;
    }

    public async Task ChangeUserCounterAsync(long userId, int number, Update update, CancellationToken cancellationToken)
    {
        var chat = await _dbContext.Chats
            .Include(c => c.UserCounters)
            .FirstAsync(c => c.ChatId == update.Message.Chat.Id, cancellationToken);
        var counter = chat
            .UserCounters
            .First(c => c.UserId == userId && c.ChatId == chat.ChatId);
        counter.Counter = number;
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<List<UserChatCounter>> GetStatisticsAsync(Update update, CancellationToken cancellationToken)
    {
        return await _dbContext.Counters
            .Where(c => c.ChatId == update.Message.Chat.Id)
            .Include(c => c.User)
            .OrderByDescending(c => c.Counter)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> CanRunTodayAsync(Update update, CancellationToken cancellationToken)
    {
        var lastRunAt = await _dbContext
            .RunLogs
            .Where(r => r.ChatId == update.Message.Chat.Id)
            .Select(r => r.LastRunAt)
            .FirstAsync();
        return lastRunAt != DateOnly.FromDateTime(DateTime.UtcNow);
    }

    public async Task SetRunDateAsync(Update update, CancellationToken cancellationToken)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var record = await _dbContext.RunLogs.FindAsync(update.Message.Chat.Id, cancellationToken);
        record.LastRunAt = today;
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task InitializeRunDateAsync(Update update, CancellationToken cancellationToken)
    {
        _dbContext.RunLogs.Add(new ChatRunLog() { ChatId = update.Message.Chat.Id, 
            LastRunAt = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-100)) });
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}