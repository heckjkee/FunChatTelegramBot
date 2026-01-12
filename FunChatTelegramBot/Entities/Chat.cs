using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace FunChatTelegramBot;

public class Chat
{
    public long ChatId { get; set; }
    public ICollection<UserChatCounter> UserCounters { get; set; }
}