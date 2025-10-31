using AlpimiAPI.Locales;
using Microsoft.Extensions.Localization;

namespace AlpimiAPI.Utilities
{
    public static class DayOfWeek
    {
        public static Dictionary<string, int> dayToIntMap =
            new(StringComparer.OrdinalIgnoreCase)
            {
                { "Sunday", 0 },
                { "Monday", 1 },
                { "Tuesday", 2 },
                { "Wednesday", 3 },
                { "Thursday", 4 },
                { "Friday", 5 },
                { "Saturday", 6 },
                //
                { "Niedziela", 0 },
                { "Poniedziałek", 1 },
                { "Wtorek", 2 },
                { "Środa", 3 },
                { "Czwartek", 4 },
                { "Piątek", 5 },
                { "Sobota", 6 }
            };

        public static string intToDayMap(int dayInt, IStringLocalizer<Data> _strData)
        {
            return dayInt switch
            {
                0 => _strData["Sunday"],
                1 => _strData["Monday"],
                2 => _strData["Tuesday"],
                3 => _strData["Wednesday"],
                4 => _strData["Thursday"],
                5 => _strData["Friday"],
                6 => _strData["Saturday"],
                _ => ""
            };
        }
    }
}
