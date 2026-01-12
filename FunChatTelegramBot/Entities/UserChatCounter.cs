namespace FunChatTelegramBot;

public class UserChatCounter
{
    public int Id { get; set; }
    public long UserId { get; set; }
    public long ChatId { get; set; }
    public User User { get; set; }
    public Chat Chat { get; set; }
    public int Counter { get; set; }
}