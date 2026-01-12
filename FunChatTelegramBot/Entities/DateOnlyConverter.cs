using System.Globalization;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace FunChatTelegramBot;

public class DateOnlyConverter : ValueConverter<DateOnly, string>
{
    public DateOnlyConverter()
        : base(d => d.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
            s => DateOnly.Parse(s, CultureInfo.InvariantCulture)) { }
}